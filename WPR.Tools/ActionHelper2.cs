using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WPR.Tools;

#region Инфраструктура

/// <summary>
/// Цепочка действий. Прерывается по предикатам. Можно исполнять много раз (последовательно).
/// Параллельные одновременные запуски одного экземпляра не поддерживаются
/// </summary>
public interface IActionChain
{
    /// <summary>
    /// Выполнить все шаги по очереди.
    /// Если какой-либо шаг вернул false — дальнейшие шаги не выполняются.
    /// Если шаг выбросил исключение — выполнение прекращается, исключение уходит вверх.
    /// </summary>
    Task<bool> ExecuteAsync(CancellationToken cancel = default);
}

/// <summary>
/// Базовый интерфейс шага цепочки.
/// Возвращает true, если нужно продолжать выполнение, false — если цепочку надо прервать.
/// Любое исключение *не перехватывается* и прерывает цепочку (пусть это увидит вызывающий код).
/// </summary>
internal interface IChainStep
{
    Task<bool> ExecuteAsync(CancellationToken cancel);
    void Reset(); // Сброс состояния перед каждым новым запуском цепочки
}

/// <summary>
/// Цепочка действий. Можно исполнять много раз (последовательно).
/// Параллельные одновременные запуски одного экземпляра не поддерживаются (см. комментарии ниже).
/// </summary>
internal sealed class ActionChain(IEnumerable<IChainStep> steps) : IActionChain
{
    private readonly List<IChainStep> _Steps = [.. steps];

    private bool _IsRunning;

    internal void Add(IChainStep step) => _Steps.Add(step);

    /// <summary>
    /// Выполнить все шаги по очереди.
    /// Если какой-либо шаг вернул false — дальнейшие шаги не выполняются.
    /// Если шаг выбросил исключение — выполнение прекращается, исключение уходит вверх.
    /// </summary>
    public async Task<bool> ExecuteAsync(CancellationToken cancel = default)
    {
        if (_IsRunning)
            throw new InvalidOperationException("Одновременое выполнение не поддерживается!");

        _IsRunning = true;

        try
        {
            // Сбросить состояние всех шагов, чтобы повторные запуски были «чистыми»
            foreach (var step in _Steps)
                step.Reset();

            foreach (var step in _Steps)
            {
                cancel.ThrowIfCancellationRequested();
                var shouldContinue = await step.ExecuteAsync(cancel).ConfigureAwait(false);

                if (!shouldContinue)
                {
                    return false;
                }
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        finally
        {
            _IsRunning = false;
        }
    }
}

#endregion

#region Шаги

/// <summary>
/// Шаг-проверка. Если условие ложно — опционально вызываем onFail и прерываем цепочку.
/// </summary>
internal sealed class CheckStep(
    Func<CancellationToken, Task<bool>> Condition) : IChainStep
{
    private readonly Func<CancellationToken, Task<bool>> _Condition = Condition ?? throw new ArgumentNullException(nameof(Condition));

    public Func<CancellationToken, Task>? OnFail { get; set; }
    public Func<CancellationToken, Task>? OnSuccess { get; set; }

    public void Reset() { /* состояния нет */ }

    public async Task<bool> ExecuteAsync(CancellationToken cancel)
    {
        var result = await _Condition.Invoke(cancel).ConfigureAwait(false);
        if (result)
            OnSuccess?.Invoke(cancel).ConfigureAwait(false);
        else
            OnFail?.Invoke(cancel).ConfigureAwait(false);

        return result;
    }
}

/// <summary>
/// Шаг: синхронное действие без результата (void).
/// </summary>
internal sealed class ThenVoidStep(Action Action) : IChainStep
{
    private readonly Action _Action = Action ?? throw new ArgumentNullException(nameof(Action));

    public void Reset() { /* состояния нет */ }

    public Task<bool> ExecuteAsync(CancellationToken cancel)
    {
        _Action();
        return Task.FromResult(true);
    }
}

/// <summary>
/// Шаг: асинхронное действие без результата (Task).
/// </summary>
internal sealed class ThenTaskStep(Func<CancellationToken, Task> Action) : IChainStep
{
    private readonly Func<CancellationToken, Task> _Action = Action ?? throw new ArgumentNullException(nameof(Action));

    public void Reset() { /* состояния нет */ }

    public async Task<bool> ExecuteAsync(CancellationToken cancel)
    {
        await _Action(cancel).ConfigureAwait(false);
        return true;
    }
}

/// <summary>
/// Шаг: асинхронное действие с результатом T и предикатом успешности.
/// Если предикат вернул false — шаг считается провальным, вызывается OnFail (если задан), цепочка прерывается.
/// Если предикат вернул true — вызывается OnSuccess (если задан), цепочка продолжается.
/// </summary>
internal sealed class ThenStep<T>(Func<CancellationToken, Task<T>> Action, Predicate<T>? Predicate) : IChainStep
{
    private readonly Func<CancellationToken, Task<T>> _Action = Action ?? throw new ArgumentNullException(nameof(Action));

    // Обработчики результата. Устанавливаются билдером.
    public Func<T,CancellationToken, Task>? OnFail { get; set; }
    public Func<T, CancellationToken, Task>? OnSuccess { get; set; }

    // Результат последнего выполнения этого шага (используется, например, в ThenIf)
    public T? Result { get; private set; }

    public void Reset()
    {
        Result = default;
    }

    public async Task<bool> ExecuteAsync(CancellationToken cancel)
    {
        var result = await _Action(cancel).ConfigureAwait(false);
        Result = result;

        var ok = Predicate?.Invoke(result) ?? true;

        if (!ok)
            if (OnFail != null) await OnFail(result, cancel).ConfigureAwait(false);
        else
            if (OnSuccess != null) await OnSuccess(result, cancel).ConfigureAwait(false);

        return ok;
    }
}

#endregion

#region Билдеры

/// <summary>
/// Содержит список цепочек для выполнения.
/// Чтобы метод Build возвращал новую цепочку для использования в многопотоке
/// </summary>
internal class BuilderContext
{
    private readonly IList<IChainStep> _Steps = [];

    public void Add(IChainStep step) => _Steps.Add(step);

    public IEnumerable<IChainStep> Steps => _Steps;

}

/// <summary>
/// Билдер «нетипизированной» части цепочки.
/// Позволяет добавлять void/Task шаги и переходить к типизированным шагам через Then&lt;T&gt;.
/// </summary>
public class ActionBuilder
{
    internal BuilderContext Context { get; }

    internal ActionBuilder(BuilderContext context) => Context = context ?? throw new ArgumentNullException(nameof(context));


    /// <summary>
    /// Синхронное действие без результата (void).
    /// </summary>
    public ActionBuilder Then(Action action)
    {
        Context.Add(new ThenVoidStep(action));
        return this;
    }

    /// <summary>
    /// Асинхронное действие без результата (Task).
    /// </summary>
    public ActionBuilder Then(Func<CancellationToken, Task> action)
    {
        Context.Add(new ThenTaskStep(action));
        return this;
    }

    /// <summary>
    /// Асинхронное действие с результатом T и предикатом успешности.
    /// </summary>
    public ActionBuilder<T> Then<T>(Func<CancellationToken, Task<T>> action, Predicate<T>? predicate = null)
    {
        var step = new ThenStep<T>(action, predicate);
        Context.Add(step);
        return new ActionBuilder<T>(Context, step);
    }

    /// <summary>
    /// Начальная или промежуточная асинхронная проверка без типизации результата.
    /// </summary>
    public CheckActionBuilder Check(Func<CancellationToken, Task<bool>> condition)
    {
        var step = new CheckStep(condition);
        Context.Add(step);
        return new CheckActionBuilder(Context, step);
    }

    /// <summary>
    /// Начальная или промежуточная синхронная проверка без типизации результата.
    /// </summary>
    public CheckActionBuilder Check(Func<bool> condition)
    {
        var step = new CheckStep(_ => Task.FromResult(condition.Invoke()));
        Context.Add(step);
        return new CheckActionBuilder(Context, step);
    }

    /// <summary>
    /// Выполнить цепочку.
    /// </summary>
    public Task<bool> ExecuteAsync(CancellationToken token = default) => Build().ExecuteAsync(token);


    /// <summary>
    /// Получить цепочку для дальнейшего использования
    /// </summary>
    public IActionChain Build() => new ActionChain(Context.Steps);
}

public sealed class CheckActionBuilder : ActionBuilder
{
    private readonly CheckStep _Step;

    internal CheckActionBuilder(BuilderContext context, CheckStep step) : base(context)
    {
        _Step = step;
    }

    /// <summary>
    /// Зарегистрировать обработчик неуспеха этого шага.
    /// </summary>
    public ActionBuilder OnFail(Func<CancellationToken, Task> onFail)
    {
        _Step.OnFail = onFail ?? throw new ArgumentNullException(nameof(onFail));
        return this;
    }

    /// <summary>
    /// Зарегистрировать синхронный обработчик неуспеха этого шага.
    /// </summary>
    public ActionBuilder OnFail(Action onFail)
    {
        _Step.OnFail = onFail is null ? throw new ArgumentNullException(nameof(onFail)): _ =>
        {
            onFail.Invoke();
            return Task.CompletedTask;
        };
        return this;
    }

    /// <summary>
    /// Зарегистрировать обработчик успеха этого шага.
    /// </summary>
    public ActionBuilder OnSuccess(Func<CancellationToken, Task> onSuccess)
    {
        _Step.OnSuccess = onSuccess ?? throw new ArgumentNullException(nameof(onSuccess));
        return this;
    }

    /// <summary>
    /// Зарегистрировать синхронный обработчик успеха этого шага.
    /// </summary>
    public ActionBuilder OnSuccess(Action onSuccess)
    {
        _Step.OnSuccess = onSuccess is null ? throw new ArgumentNullException(nameof(onSuccess)) : _ =>
        {
            onSuccess.Invoke();
            return Task.CompletedTask;
        };
        return this;
    }

}

/// <summary>
/// Типизированный билдер, «привязанный» к шагу ThenStep&lt;T&gt;.
/// Позволяет повесить OnSuccess/OnFail для этого шага и строить ветвления на основе его результата.
/// </summary>
public sealed class ActionBuilder<T> : ActionBuilder
{
    private readonly ThenStep<T> _Step;

    internal ActionBuilder(BuilderContext context, ThenStep<T> step) : base(context)
    {
        _Step = step ?? throw new ArgumentNullException(nameof(step));
    }

    /// <summary>
    /// Зарегистрировать обработчик неуспеха этого шага (predicate == false).
    /// </summary>
    public ActionBuilder<T> OnFail(Func<T, CancellationToken, Task> onFail)
    {
        _Step.OnFail = onFail ?? throw new ArgumentNullException(nameof(onFail));
        return this;
    }

    /// <summary>
    /// Зарегистрировать синхронный обработчик неуспеха этого шага (predicate == false).
    /// </summary>
    public ActionBuilder<T> OnFail(Action<T> onFail)
    {
        _Step.OnFail = onFail is null ? throw new ArgumentNullException(nameof(onFail)) : (t, _) =>
        {
            onFail.Invoke(t);
            return Task.CompletedTask;
        };
        return this;
    }

    /// <summary>
    /// Зарегистрировать обработчик успеха этого шага (predicate == true).
    /// </summary>
    public ActionBuilder<T> OnSuccess(Func<T, CancellationToken, Task> onSuccess)
    {
        _Step.OnSuccess = onSuccess ?? throw new ArgumentNullException(nameof(onSuccess));
        return this;
    }


    /// <summary>
    /// Зарегистрировать синхронный обработчик успеха этого шага (predicate == true).
    /// </summary>
    public ActionBuilder<T> OnSuccess(Action<T> onSuccess)
    {
        _Step.OnSuccess = onSuccess is null ? throw new ArgumentNullException(nameof(onSuccess)) : (t, _) =>
        {
            onSuccess.Invoke(t);
            return Task.CompletedTask;
        };
        return this;
    }

    /// <summary>
    /// Выполнить следующий типизированный шаг с использованием результата предыдущего
    /// </summary>
    public ActionBuilder<TNew> ThenWithResult<TNew>(Func<T, CancellationToken, Task<TNew>> action, Predicate<TNew>? predicate = null)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        var next = new ThenStep<TNew>(token => action.Invoke(_Step.Result!, token), predicate);
        Context.Add(next);
        return new ActionBuilder<TNew>(Context, next);
    }

    /// <summary>
    /// Выполнить следующий типизированный шаг синхронно с использованием результата предыдущего
    /// </summary>
    public ActionBuilder<TNew> ThenWithResult<TNew>(Func<T, TNew> action, Predicate<TNew>? predicate = null)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        var next = new ThenStep<TNew>(_ =>
        {
            var result = action.Invoke(_Step.Result!);
            return Task.FromResult(result);
        }, predicate);

        Context.Add(next);
        return new ActionBuilder<TNew>(Context, next);
    }

    public ActionBuilder ThenWithResult(Action<T> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        var next = new ThenVoidStep(() => action.Invoke(_Step.Result!));
        Context.Add(next);
        return new ActionBuilder(Context);
    }

    public ActionBuilder ThenWithResult(Func<T, CancellationToken, Task<T>> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        var next = new ThenTaskStep(ct => action.Invoke(_Step.Result!, ct));
        Context.Add(next);
        return new ActionBuilder(Context);
    }
}

#endregion

#region Фасад

/// <summary>
/// Помощник запуска цепочек действий и задач
/// </summary>
public class ActionHelper2
{
    /// <summary>
    /// Действие, выполняемое перед каждым запуском цепочки
    /// </summary>
    public Action? StarAction { get; set; }

    /// <summary>
    /// Действие, выполняемое после выполнения цепочки с любым результатом
    /// Подставляет результат выполнения цепочки
    /// </summary>
    public Action<bool>? EndAction { get; set; }

    /// <summary>
    /// Если задано - будет вызвано при исключении любого типа.
    /// Если не задано - ошибки ловиться не будут
    /// </summary>
    public Action<Exception>? OnExceptionAction { get; set; }

    /// <summary>
    /// Начать построение новой цепочки.
    /// </summary>
    public static ActionBuilder Start() => new(new BuilderContext());

    /// <summary>
    /// Начать построение новой цепочки с начальной проверки.
    /// </summary>
    public static ActionBuilder Check(Func<bool> condition) => Start().Check(condition);

    /// <summary>
    /// Начать построение новой цепочки с начальной асинхронной проверки.
    /// </summary>
    public static ActionBuilder Check(Func<CancellationToken, Task<bool>> condition) => Start().Check(condition);

    /// <summary>
    /// Запустить готовую цепочку.
    /// </summary>
    public async Task<bool> ExecuteAsync(IActionChain chain, CancellationToken cancel = default)
    {
        StarAction?.Invoke();
        var executingResult = false;
        try
        {
            executingResult = await chain.ExecuteAsync(cancel);
            return executingResult;
        }
        catch (Exception ex)
        {
            if (OnExceptionAction != null)
                OnExceptionAction(ex);
            else
                throw;
        }
        finally
        {
            EndAction?.Invoke(executingResult);
        }

        return executingResult;
    }

    /// <summary>
    /// Запустить цепочку, возвращаясь от билдера.
    /// </summary>
    public Task<bool> ExecuteAsync(ActionBuilder builder, CancellationToken cancel = default) => ExecuteAsync(builder.Build(), cancel);
}

#endregion