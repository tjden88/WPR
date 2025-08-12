using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WPR.Tools;

using System.Diagnostics;

/// <summary>
/// Потокобезопасный помощник для запуска отменяемых задач.
/// Каждый новый запуск отменяет предыдущую и ждёт её завершения.
/// Работает без лишних блокировок, используя атомарные операции Interlocked.
/// </summary>
public class TaskRunner
{
    private CancellationTokenSource? _cts;
    private Task? _currentTask;

    /// <summary>
    /// Выполняется ли сейчас задача.
    /// </summary>
    public bool IsRunning => _currentTask is { IsCompleted: false };

    /// <summary>
    /// Запускает новую задачу, предварительно отменив предыдущую.
    /// </summary>
    public async Task Start(Func<CancellationToken, Task> work)
    {
        var newCts = new CancellationTokenSource();
        var oldCts = Interlocked.Exchange(ref _cts, newCts);

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

        var oldTask = Interlocked.Exchange(ref _currentTask, null);
        if (oldTask is not null)
        {
            try
            {
                await oldTask;
            }
            catch (OperationCanceledException) { }
        }

        var task = Task.Run(() => work(newCts.Token), newCts.Token);
        _currentTask = task;

        try
        {
            await task;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
        finally
        {
            Interlocked.CompareExchange(ref _cts, null, newCts);
            newCts.Dispose();
        }
    }

    /// <summary>
    /// Принудительно отменяет текущую задачу и ждёт её завершения.
    /// </summary>
    public async Task Cancel()
    {
        var oldCts = Interlocked.Exchange(ref _cts, null);
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

        var oldTask = Interlocked.Exchange(ref _currentTask, null);
        if (oldTask is not null)
        {
            try
            {
                await oldTask;
            }
            catch (OperationCanceledException) { }
        }
    }
}
