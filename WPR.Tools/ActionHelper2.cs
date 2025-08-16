using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WPR.Tools
{
    #region Инфраструктура

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
    public sealed class ActionChain
    {
        private readonly List<IChainStep> _steps = new();

        internal void Add(IChainStep step) => _steps.Add(step);

        /// <summary>
        /// Выполнить все шаги по очереди.
        /// Если какой-либо шаг вернул false — дальнейшие шаги не выполняются.
        /// Если шаг выбросил исключение — выполнение прекращается, исключение уходит вверх.
        /// </summary>
        public async Task ExecuteAsync(CancellationToken cancel = default)
        {
            // Сбросить состояние всех шагов, чтобы повторные запуски были «чистыми»
            foreach (var step in _steps)
                step.Reset();

            foreach (var step in _steps)
            {
                cancel.ThrowIfCancellationRequested();
                var shouldContinue = await step.ExecuteAsync(cancel).ConfigureAwait(false);
                if (!shouldContinue)
                    break;
            }
        }
    }

    #endregion

    #region Шаги

    /// <summary>
    /// Шаг-проверка. Если условие ложно — опционально вызываем onFail и прерываем цепочку.
    /// </summary>
    internal sealed class CheckStep : IChainStep
    {
        private readonly Func<bool> _condition;
        private readonly Func<Task>? _onFail;

        public CheckStep(Func<bool> condition, Func<Task>? onFail)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _onFail = onFail;
        }

        public void Reset() { /* состояния нет */ }

        public async Task<bool> ExecuteAsync(CancellationToken cancel)
        {
            if (_condition()) return true;

            if (_onFail != null)
                await _onFail().ConfigureAwait(false);

            return false; // прервать цепочку
        }
    }

    /// <summary>
    /// Шаг: синхронное действие без результата (void).
    /// </summary>
    internal sealed class ThenVoidStep : IChainStep
    {
        private readonly Action _action;

        public ThenVoidStep(Action action) =>
            _action = action ?? throw new ArgumentNullException(nameof(action));

        public void Reset() { /* состояния нет */ }

        public Task<bool> ExecuteAsync(CancellationToken cancel)
        {
            _action();
            return Task.FromResult(true);
        }
    }

    /// <summary>
    /// Шаг: асинхронное действие без результата (Task).
    /// </summary>
    internal sealed class ThenTaskStep : IChainStep
    {
        private readonly Func<CancellationToken, Task> _action;

        public ThenTaskStep(Func<CancellationToken, Task> action) =>
            _action = action ?? throw new ArgumentNullException(nameof(action));

        public void Reset() { /* состояния нет */ }

        public async Task<bool> ExecuteAsync(CancellationToken cancel)
        {
            await _action(cancel).ConfigureAwait(false);
            return true;
        }
    }

    /// <summary>
    /// Шаг: асинхронное действие с результатом T и предикатом успешности.
    /// Если предикат вернул false — шаг считается провальным, вызывается OnFail (если задан), цепочка прерывается.
    /// Если предикат вернул true — вызывается OnSuccess (если задан), цепочка продолжается.
    /// </summary>
    internal sealed class ThenStep<T> : IChainStep
    {
        private readonly Func<CancellationToken, Task<T>> _action;
        private readonly Predicate<T>? _predicate;

        // Обработчики результата. Устанавливаются билдером.
        public Func<T, Task>? OnFail { get; set; }
        public Func<T, Task>? OnSuccess { get; set; }

        // Результат последнего выполнения этого шага (используется, например, в ThenIf)
        public T? Result { get; private set; }

        public ThenStep(Func<CancellationToken, Task<T>> action, Predicate<T>? predicate)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _predicate = predicate;
        }

        public void Reset()
        {
            Result = default;
        }

        public async Task<bool> ExecuteAsync(CancellationToken cancel)
        {
            // 1) Выполняем действие и сохраняем результат
            var result = await _action(cancel).ConfigureAwait(false);
            Result = result;

            // 2) Определяем, успешен ли шаг
            var ok = _predicate?.Invoke(result) ?? true;

            if (!ok)
            {
                // 3a) Неуспех: вызвать OnFail (если задан) и остановить цепочку
                if (OnFail != null)
                    await OnFail(result).ConfigureAwait(false);

                return false;
            }

            // 3b) Успех: вызвать OnSuccess (если задан) и продолжать
            if (OnSuccess != null)
                await OnSuccess(result).ConfigureAwait(false);

            return true;
        }
    }

    #endregion

    #region Билдеры

    /// <summary>
    /// Билдер «нетипизированной» части цепочки.
    /// Позволяет добавлять void/Task шаги и переходить к типизированным шагам через Then&lt;T&gt;.
    /// </summary>
    public class ActionBuilder
    {
        protected ActionChain Chain { get; }

        internal ActionBuilder(ActionChain chain) =>
            Chain = chain ?? throw new ArgumentNullException(nameof(chain));

        /// <summary>
        /// Синхронное действие без результата (void).
        /// </summary>
        public ActionBuilder Then(Action action)
        {
            Chain.Add(new ThenVoidStep(action));
            return this;
        }

        /// <summary>
        /// Асинхронное действие без результата (Task).
        /// </summary>
        public ActionBuilder Then(Func<CancellationToken, Task> action)
        {
            Chain.Add(new ThenTaskStep(action));
            return this;
        }

        /// <summary>
        /// Асинхронное действие с результатом T и предикатом успешности.
        /// </summary>
        public ActionBuilder<T> Then<T>(Func<CancellationToken, Task<T>> action, Predicate<T>? predicate = null)
        {
            var step = new ThenStep<T>(action, predicate);
            Chain.Add(step);
            return new ActionBuilder<T>(Chain, step);
        }

        /// <summary>
        /// Начальная проверка (или промежуточная) без типизации результата.
        /// Если условие ложно — вызывается onFail (если задан), выполнение цепочки прерывается.
        /// </summary>
        public ActionBuilder Check(Func<bool> condition, Func<Task>? onFail = null)
        {
            Chain.Add(new CheckStep(condition, onFail));
            return this;
        }

        /// <summary>
        /// Выполнить цепочку.
        /// </summary>
        public Task ExecuteAsync(CancellationToken token = default) => Chain.ExecuteAsync(token);
    }

    /// <summary>
    /// Типизированный билдер, «привязанный» к шагу ThenStep&lt;T&gt;.
    /// Позволяет повесить OnSuccess/OnFail для этого шага и строить ветвления на основе его результата.
    /// </summary>
    public sealed class ActionBuilder<T> : ActionBuilder
    {
        private readonly ThenStep<T> _step;

        internal ActionBuilder(ActionChain chain, ThenStep<T> step) : base(chain)
        {
            _step = step ?? throw new ArgumentNullException(nameof(step));
        }

        /// <summary>
        /// Зарегистрировать обработчик неуспеха этого шага (predicate == false).
        /// </summary>
        public ActionBuilder<T> OnFail(Func<T, Task> onFail)
        {
            _step.OnFail = onFail ?? throw new ArgumentNullException(nameof(onFail));
            return this;
        }

        /// <summary>
        /// Зарегистрировать обработчик успеха этого шага (predicate == true).
        /// </summary>
        public ActionBuilder<T> OnSuccess(Func<T, Task> onSuccess)
        {
            _step.OnSuccess = onSuccess ?? throw new ArgumentNullException(nameof(onSuccess));
            return this;
        }

        /// <summary>
        /// Ветвление: проверить условие над результатом этого шага.
        /// Если условие ложно — вызвать onFail и прервать цепочку.
        /// Если истинно — выполнить следующий типизированный шаг.
        /// </summary>
        public ActionBuilder<TU> ThenIf<TU>(
            Predicate<T> condition,
            Func<T, Task> onFail,
            Func<CancellationToken, Task<TU>> action,
            Predicate<TU>? predicate = null)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (onFail == null) throw new ArgumentNullException(nameof(onFail));
            if (action == null) throw new ArgumentNullException(nameof(action));

            // Промежуточная проверка, использующая результат текущего шага
            Chain.Add(new CheckStep(
                condition: () => condition(_step.Result!),
                onFail: () => onFail(_step.Result!)
            ));

            var next = new ThenStep<TU>(action, predicate);
            Chain.Add(next);
            return new ActionBuilder<TU>(Chain, next);
        }
    }

    #endregion

    #region Статический фасад

    /// <summary>
    /// Статический фасад для удобного старта/запуска цепочек.
    /// </summary>
    public static class ActionHelper
    {
        /// <summary>
        /// Начать построение новой цепочки без начальной проверки.
        /// </summary>
        public static ActionBuilder Start()
        {
            var chain = new ActionChain();
            return new ActionBuilder(chain);
        }

        /// <summary>
        /// Начать построение новой цепочки с начальной проверки.
        /// </summary>
        public static ActionBuilder Check(Func<bool> condition, Func<Task>? onFail = null)
            => Start().Check(condition, onFail);

        /// <summary>
        /// Запустить готовую цепочку.
        /// </summary>
        public static Task ExecuteAsync(ActionChain chain, CancellationToken cancel = default)
            => chain.ExecuteAsync(cancel);

        /// <summary>
        /// Запустить цепочку, возвращаясь от билдера.
        /// </summary>
        public static Task ExecuteAsync(ActionBuilder builder, CancellationToken cancel = default)
            => builder.ExecuteAsync(cancel);
    }

    #endregion

    #region Пример модели результата (опционально)

    /// <summary>
    /// Пример простой модели результата. Используйте свою, если она уже есть в проекте.
    /// </summary>
    public sealed class OperationResult
    {
        public bool IsSuccess { get; }
        public string? Error { get; }

        private OperationResult(bool isSuccess, string? error = null)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static OperationResult Success() => new(true);
        public static OperationResult Failure(string? error = null) => new(false, error);
    }

    #endregion
}
