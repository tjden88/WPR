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
        if(onBusyStateChanged is not null)
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