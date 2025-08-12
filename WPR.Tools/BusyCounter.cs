using System;
using System.Threading;

namespace WPR.Tools;

/// <summary>
/// Потокобезопасный счётчик "занятости".
/// Подходит для индикации состояния (например, загрузки данных) при выполнении асинхронных операций.
/// </summary>
public sealed class BusyCounter
{
    public BusyCounter(Action<bool>? onBusyStateChanged = null)
    {
        if (onBusyStateChanged is not null)
            BusyStateChanged += onBusyStateChanged;
    }

    private int _Counter; // Текущее количество активных операций

    /// <summary>
    /// Событие вызывается при изменении состояния "занят / свободен".
    /// Передаваемый параметр — true, если есть хотя бы одна активная операция, false — если нет.
    /// </summary>
    public event Action<bool>? BusyStateChanged;

    /// <summary>
    /// True, если выполняется хотя бы одна операция.
    /// </summary>
    public bool IsBusy => _Counter > 0;

    /// <summary>
    /// Увеличивает счётчик активных операций.
    /// Вызывает событие <see cref="BusyStateChanged"/>, если мы перешли из состояния "свободен" в "занят".
    /// </summary>
    public void Enter()
    {
        var newValue = Interlocked.Increment(ref _Counter);
        if (newValue == 1) // Было 0, стало 1 → начали работать
            BusyStateChanged?.Invoke(true);
    }

    /// <summary>
    /// Уменьшает счётчик активных операций.
    /// Вызывает событие <see cref="BusyStateChanged"/>, если мы вернулись в состояние "свободен".
    /// </summary>
    /// <exception cref="InvalidOperationException">Если вызвано больше раз, чем Enter()</exception>
    public void Exit()
    {
        var newValue = Interlocked.Decrement(ref _Counter);
        if (newValue < 0)
        {
            Interlocked.Exchange(ref _Counter, 0);
            throw new InvalidOperationException("Вызов Exit() без соответствующего Enter().");
        }

        if (newValue == 0) // Было 1, стало 0 → закончили работу
            BusyStateChanged?.Invoke(false);
    }

    /// <summary>
    /// Вспомогательный метод для безопасного использования в конструкции using:
    /// using (busyCounter.BeginScope()) { ... }
    /// </summary>
    public IDisposable BeginScope()
    {
        Enter();
        return new Scope(this);
    }

    private sealed class Scope(BusyCounter owner) : IDisposable
    {
        private BusyCounter? _Owner = owner;

        public void Dispose()
        {
            _Owner?.Exit();
            _Owner = null;
        }
    }
}

/// <summary>
/// Потокобезопасный счётчик "занятости" с булевым флагом IsBusy и дополнительным состоянием типа TState.
/// Позволяет уведомлять о наличии активных операций и передавать дополнительную информацию.
/// </summary>
/// <typeparam name="TState">Тип дополнительного состояния.</typeparam>
public sealed class BusyCounter<TState>
{
    private int _counter; // Счётчик активных операций
    private TState _currentState = default!; // Текущее дополнительное состояние

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="onBusyStateChanged">Опциональный обработчик события изменения состояния.</param>
    public BusyCounter(Action<bool, TState>? onBusyStateChanged = null)
    {
        if (onBusyStateChanged != null)
            BusyStateChanged += onBusyStateChanged;
    }

    /// <summary>
    /// Событие вызывается при изменении занятости.
    /// Параметры:
    /// bool isBusy — true, если есть активные операции,
    /// TState state — дополнительное состояние (например, описание текущей загрузки).
    /// </summary>
    public event Action<bool, TState>? BusyStateChanged;

    /// <summary>
    /// Флаг, показывает, есть ли активные операции.
    /// </summary>
    public bool IsBusy => _counter > 0;

    /// <summary>
    /// Увеличивает счётчик активных операций с передачей дополнительного состояния.
    /// Вызывает событие, если мы перешли из состояния "свободен" в "занят",
    /// или обновляем состояние при продолжении работы.
    /// </summary>
    /// <param name="state">Дополнительное состояние для текущей операции.</param>
    public void Enter(TState state)
    {
        var newValue = Interlocked.Increment(ref _counter);
        _currentState = state;
        BusyStateChanged?.Invoke(true, _currentState);
    }

    /// <summary>
    /// Уменьшает счётчик активных операций.
    /// Если операций больше не осталось, вызывает событие с isBusy = false и default состоянием.
    /// Выбрасывает исключение, если вызвано больше раз, чем Enter().
    /// </summary>
    public void Exit()
    {
        var newValue = Interlocked.Decrement(ref _counter);
        if (newValue < 0)
        {
            Interlocked.Exchange(ref _counter, 0);
            _currentState = default!;
            throw new InvalidOperationException("Вызов Exit() без соответствующего Enter().");
        }

        if (newValue == 0)
        {
            // Свободен, сбрасываем состояние
            _currentState = default!;
            BusyStateChanged?.Invoke(false, _currentState);
        }
        else
        {
            // Всё ещё занят, но может состояние обновлять не нужно (оставим как есть)
            BusyStateChanged?.Invoke(true, _currentState);
        }
    }

    /// <summary>
    /// Вспомогательный метод для использования через using.
    /// Увеличивает счётчик и устанавливает состояние при входе,
    /// уменьшает счётчик при выходе.
    /// </summary>
    /// <param name="state">Дополнительное состояние для текущей операции.</param>
    /// <returns>IDisposable, который при Dispose вызовет Exit().</returns>
    public IDisposable BeginScope(TState state)
    {
        Enter(state);
        return new Scope(this);
    }

    private sealed class Scope : IDisposable
    {
        private BusyCounter<TState>? _owner;

        public Scope(BusyCounter<TState> owner)
        {
            _owner = owner;
        }

        public void Dispose()
        {
            _owner?.Exit();
            _owner = null;
        }
    }
}