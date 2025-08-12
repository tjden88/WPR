using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WPR.Tools;

/// <summary>
/// Потокобезопасный помощник для запуска отменяемых задач.
/// Каждый новый запуск отменяет предыдущую и ждёт её завершения.
/// Работает без лишних блокировок, используя атомарные операции Interlocked.
/// </summary>
public class TaskRunner
{
    // Текущий токен отмены
    private CancellationTokenSource? _cts;

    // Текущая выполняемая задача
    private Task? _currentTask;

    /// <summary>
    /// Запускает новую задачу, предварительно отменив предыдущую.
    /// </summary>
    public async Task Start(Func<CancellationToken, Task> work)
    {
        // Создаём новый CTS и атомарно подменяем старый
        var newCts = new CancellationTokenSource();
        var oldCts = Interlocked.Exchange(ref _cts, newCts);

        // Отменяем и освобождаем старый CTS (если был)
        if (oldCts is not null)
        {
            try
            {
                await oldCts.CancelAsync();
            }
            catch (ObjectDisposedException)
            {
                // Уже освобождён — игнорируем
            }
            finally
            {
                oldCts.Dispose();
            }
        }

        // Ждём завершения предыдущей задачи (если была)
        var oldTask = Interlocked.Exchange(ref _currentTask, null);
        if (oldTask is not null)
        {
            try
            {
                await oldTask;
            }
            catch (OperationCanceledException)
            {
                // Ожидаемая отмена — игнорируем
            }
        }

        // Запускаем новую задачу
        var task = Task.Run(() => work(newCts.Token), newCts.Token);

        // Сохраняем ссылку на неё
        _currentTask = task;

        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            // Нормальная отмена
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
        finally
        {
            // Сбрасываем CTS только если он всё ещё наш
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