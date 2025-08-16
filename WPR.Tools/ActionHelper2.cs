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
    private readonly List<IChainStep> _Steps = [..steps];

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
internal sealed class CheckStep(Func<bool> Condition, Func<Task>? OnFail) : IChainStep
{
    private readonly Func<bool> _Condition = Condition ?? throw new ArgumentNullException(nameof(Condition));

    public void Reset() { /* состояния нет */ }

    public async Task<bool> ExecuteAsync(CancellationToken cancel)
    {
        if (_Condition()) return true;

        if (OnFail != null)
            await OnFail().ConfigureAwait(false);

        return false; // прервать цепочку
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
    public Func<T, Task>? OnFail { get; set; }
    public Func<T, Task>? OnSuccess { get; set; }

    // Результат последнего выполнения этого шага (используется, например, в ThenIf)
    public T? Result { get; private set; }

    public void Reset()
    {
        Result = default;
    }

    public async Task<bool> ExecuteAsync(CancellationToken cancel)
    {
        // 1) Выполняем действие и сохраняем результат
        var result = await _Action(cancel).ConfigureAwait(false);
        Result = result;

        // 2) Определяем, успешен ли шаг
        var ok = Predicate?.Invoke(result) ?? true;

        if (!ok)
        {
            // 3a) Неуспех: вызвать OnFail (если задан) и остановить цепочку
            if (OnFail != null) await OnFail(result).ConfigureAwait(false);

            return false;
        }

        // 3b) Успех: вызвать OnSuccess (если задан) и продолжать
        if (OnSuccess != null) await OnSuccess(result).ConfigureAwait(false);

        return true;
    }
}

#endregion

#region Билдеры

/// <summary>
/// Содержит список цепочек для выполнения.
/// Чтобы метод Build возвращал новую цепочку для использования в многопотоке
/// </summary>
class BuilderContext
{
    private readonly List<IChainStep> _Steps = [];

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
    /// Начальная проверка (или промежуточная) без типизации результата.
    /// Если условие ложно — вызывается onFail (если задан), выполнение цепочки прерывается.
    /// </summary>
    public ActionBuilder Check(Func<bool> condition, Func<Task>? onFail = null)
    {
        Context.Add(new CheckStep(condition, onFail));
        return this;
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
    public ActionBuilder<T> OnFail(Func<T, Task> onFail)
    {
        _Step.OnFail = onFail ?? throw new ArgumentNullException(nameof(onFail));
        return this;
    }

    /// <summary>
    /// Зарегистрировать обработчик успеха этого шага (predicate == true).
    /// </summary>
    public ActionBuilder<T> OnSuccess(Func<T, Task> onSuccess)
    {
        _Step.OnSuccess = onSuccess ?? throw new ArgumentNullException(nameof(onSuccess));
        return this;
    }

    /// <summary>
    /// Ветвление: проверить условие над результатом этого шага.
    /// Если условие ложно — вызвать onFail и прервать цепочку.
    /// Если истинно — выполнить следующий типизированный шаг.
    /// </summary>
    public ActionBuilder<TU> ThenIf<TU>(
        Predicate<T> condition,
        Func<T, Task>? onFail,
        Func<CancellationToken, Task<TU>> action,
        Predicate<TU>? predicate = null)
    {
        if (condition == null) throw new ArgumentNullException(nameof(condition));
        if (action == null) throw new ArgumentNullException(nameof(action));

        // Промежуточная проверка, использующая результат текущего шага
        Context.Add(new CheckStep(() => condition(_Step.Result!), onFail is null ? null : () => onFail(_Step.Result!)));

        var next = new ThenStep<TU>(action, predicate);
        Context.Add(next);
        return new ActionBuilder<TU>(Context, next);
    }
}

#endregion

#region Статический фасад

/// <summary>
/// Помощник запуска цепочек действий и задач
/// </summary>
public static class ActionHelper2
{
    /// <summary>
    /// Начать построение новой цепочки.
    /// </summary>
    public static ActionBuilder Start() => new(new BuilderContext());

    /// <summary>
    /// Начать построение новой цепочки с начальной проверки.
    /// </summary>
    public static ActionBuilder Check(Func<bool> condition, Func<Task>? onFail = null) => Start().Check(condition, onFail);

    /// <summary>
    /// Запустить готовую цепочку.
    /// </summary>
    public static Task<bool> ExecuteAsync(IActionChain chain, CancellationToken cancel = default) => chain.ExecuteAsync(cancel);

    /// <summary>
    /// Запустить цепочку, возвращаясь от билдера.
    /// </summary>
    public static Task<bool> ExecuteAsync(ActionBuilder builder, CancellationToken cancel = default) => builder.ExecuteAsync(cancel);
}

#endregion