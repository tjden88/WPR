using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WPR.Tools;


/// <summary>
/// Контекст выполнения цепочки.
/// Хранит результаты шагов, доступные по типу.
/// </summary>
public class ActionContext
{
    private readonly Dictionary<Type, object?> _results = new();

    public void SetResult<T>(T value) => _results[typeof(T)] = value;

    public T? GetResult<T>()
    {
        if (_results.TryGetValue(typeof(T), out var val))
            return (T?)val;
        return default;
    }
}

/// <summary>
/// Интерфейс одного шага цепочки
/// </summary>
internal interface IActionStep
{
    Task<bool> ExecuteAsync(ActionContext context, CancellationToken cancel);
}

/// <summary>
/// Цепочка действий (можно выполнять много раз)
/// </summary>
public class ActionChain
{
    private readonly List<IActionStep> _steps = new();

    internal void Add(IActionStep step) => _steps.Add(step);

    public async Task ExecuteAsync(CancellationToken cancel = default)
    {
        var context = new ActionContext();

        foreach (var step in _steps)
        {
            if (!await step.ExecuteAsync(context, cancel))
                break;
        }
    }
}

/// <summary>
/// Фасад для построения цепочек
/// </summary>
public static class ActionHelper2
{
    public static ActionChain Check(Func<bool> condition, Func<Task>? OnFailMessage = null)
    {
        var chain = new ActionChain();
        chain.Add(new CheckStep(condition, OnFailMessage));
        return chain;
    }

    public static Task ExecuteAsync(ActionChain chain, CancellationToken cancel = default)
        => chain.ExecuteAsync(cancel);

    #region Внутренние шаги

    private class CheckStep : IActionStep
    {
        private readonly Func<bool> _condition;
        private readonly Func<Task>? _onFail;

        public CheckStep(Func<bool> condition, Func<Task>? onFail)
        {
            _condition = condition;
            _onFail = onFail;
        }

        public async Task<bool> ExecuteAsync(ActionContext context, CancellationToken cancel)
        {
            if (!_condition())
            {
                if (_onFail != null) await _onFail();
                return false;
            }
            return true;
        }
    }

    private class ThenStep<T> : IActionStep
    {
        private readonly Func<CancellationToken, Task<T>> _action;
        private readonly Func<T, bool>? _predicate;

        public ThenStep(Func<CancellationToken, Task<T>> action, Func<T, bool>? predicate)
        {
            _action = action;
            _predicate = predicate;
        }

        public async Task<bool> ExecuteAsync(ActionContext context, CancellationToken cancel)
        {
            T result = await _action(cancel);
            context.SetResult(result);

            if (_predicate != null && !_predicate(result))
                return false;

            return true;
        }
    }

    private class ThenSyncStep : IActionStep
    {
        private readonly Action _action;

        public ThenSyncStep(Action action) => _action = action;

        public Task<bool> ExecuteAsync(ActionContext context, CancellationToken cancel)
        {
            _action();
            return Task.FromResult(true);
        }
    }

    private class OnFailStep<T> : IActionStep
    {
        private readonly Func<T, Task> _onFail;

        public OnFailStep(Func<T, Task> onFail) => _onFail = onFail;

        public async Task<bool> ExecuteAsync(ActionContext context, CancellationToken cancel)
        {
            var result = context.GetResult<T>();

            await _onFail(result);
            return true;
        }
    }

    private class OnSuccessStep<T> : IActionStep
    {
        private readonly Func<T, Task> _onSuccess;

        public OnSuccessStep(Func<T, Task> onSuccess) => _onSuccess = onSuccess;

        public async Task<bool> ExecuteAsync(ActionContext context, CancellationToken cancel)
        {
            var result = context.GetResult<T>();
                await _onSuccess(result);
            return true;
        }
    }

    #endregion

    #region Методы-расширения для ActionChain

    public static ActionChain Then<T>(
        this ActionChain chain,
        Func<CancellationToken, Task<T>> action,
        Func<T, bool>? predicate = null)
    {
        chain.Add(new ThenStep<T>(action, predicate));
        return chain;
    }

    public static ActionChain Then(this ActionChain chain, Action action)
    {
        chain.Add(new ThenSyncStep(action));
        return chain;
    }

    public static ActionChain OnFail<T>(this ActionChain chain, Func<T, Task> onFail)
    {
        chain.Add(new OnFailStep<T>(onFail));
        return chain;
    }

    public static ActionChain OnSuccess<T>(this ActionChain chain, Func<T, Task> onSuccess)
    {
        chain.Add(new OnSuccessStep<T>(onSuccess));
        return chain;
    }

    #endregion
}
