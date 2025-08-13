using System;
using System.Threading;
using System.Threading.Tasks;
namespace WPR.Tools;

/// <summary>
/// Потокобезопасный помощник для запуска отменяемых задач (с результатом или без).
/// Каждый новый запуск отменяет предыдущий и ждёт его завершения.
/// Работает без лишних блокировок, используя атомарные операции Interlocked.
/// </summary>
public class TaskRunner
{
    private CancellationTokenSource? _Cts;
    private Task? _CurrentTask;

    /// <summary>
    /// Выполняется ли сейчас задача.
    /// </summary>
    public bool IsRunning => _CurrentTask is { IsCompleted: false };

    /// <summary>
    /// Запускает новую задачу без результата, предварительно отменив предыдущую.
    /// </summary>
    public async Task Start(Func<CancellationToken, Task> work)
    {
        await StartInternal(async token =>
        {
            await work(token);
            return true; // фиктивный результат
        });
    }


    /// <summary>
    /// Запускает новую задачу с результатом, предварительно отменив предыдущую.
    /// При отмене возвращает заданное значение.
    /// </summary>
    public async Task<T> Start<T>(Func<CancellationToken, Task<T>> work, T onCancelResult = default!)
    {
        var result = await StartInternal(work, onCancelResult);
        return result;
    }


    /// <summary>
    /// Принудительно отменяет текущую задачу и ждёт её завершения.
    /// </summary>
    public async Task Cancel()
    {
        var oldCts = Interlocked.Exchange(ref _Cts, null);
        if (oldCts is not null)
        {
            try
            {
                await oldCts.CancelAsync();
            }
            catch (ObjectDisposedException) { }
            finally
            {
                oldCts.Dispose();
            }
        }

        var oldTask = Interlocked.Exchange(ref _CurrentTask, null);
        if (oldTask is not null)
        {
            try
            {
                await oldTask;
            }
            catch (OperationCanceledException) { }
        }
    }

    // Общая реализация, которая принимает Task<T> и возвращает результат или onCancelResult при отмене
    private async Task<T> StartInternal<T>(Func<CancellationToken, Task<T>> work, T onCancelResult = default!)
    {
        var newCts = new CancellationTokenSource();
        var oldCts = Interlocked.Exchange(ref _Cts, newCts);

        // Отменяем старый CTS
        if (oldCts is not null)
        {
            try
            {
                await oldCts.CancelAsync();
            }
            catch (ObjectDisposedException) { }
            finally
            {
                oldCts.Dispose();
            }
        }

        // Ждём завершения старой задачи
        var oldTask = Interlocked.Exchange(ref _CurrentTask, null);
        if (oldTask is not null)
        {
            try
            {
                await oldTask;
            }
            catch (OperationCanceledException) { }
        }

        // Запускаем новую
        var task = Task.Run(() => work(newCts.Token), newCts.Token);
        _CurrentTask = task;

        try
        {
            return await task;
        }
        catch (OperationCanceledException)
        {
            return onCancelResult;
        }
        finally
        {
            Interlocked.CompareExchange(ref _Cts, null, newCts);
            newCts.Dispose();
        }
    }
}
