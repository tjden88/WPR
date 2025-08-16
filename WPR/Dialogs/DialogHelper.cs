using System.Diagnostics.CodeAnalysis;
using System.Windows;
using WPR.Infrastructure.Extensions;
using WPR.Theme;

namespace WPR.Dialogs;
public static class DialogHelper
{
    #region Private

    private static readonly Style _ModalWindowStyle = Application.Current?.TryFindResource("WPRModalWindow") as Style;

    // Найти панель для отображения диалога
    [return: MaybeNull]
    private static DialogRoot FindDialogPanel(DependencyObject element)
    {
        if (element == null)
            return null;

        if (element is DialogRoot panel)
            return panel;

        if (element is Window window)
            return window.Template?.FindName("WindowDialogPanel", window) as DialogRoot;

        return element.FindVisualParent<DialogRoot>();
    }

    #endregion

    #region Internal

    /// <summary>
    /// Показать диалог в потоке UI
    /// </summary>
    /// <param name="sender">Может быть Null. Объект, в котором будет найдена панель. Если не найдена - будет вызван ModalDialog</param>
    /// <param name="dialog">Диалог для показа</param>
    /// <param name="cancellationToken">Токен отмены. При вызове CancellationRequested будет возвращено false</param>
    internal static Task<bool> Show(DependencyObject sender, IWPRDialog dialog, CancellationToken cancellationToken = default)
    {
        var dispatcher = sender?.Dispatcher ?? Application.Current.Dispatcher;

        if (dispatcher == null || dispatcher.HasShutdownStarted || cancellationToken.IsCancellationRequested)
            return Task.FromResult(false);

        var tcs = new TaskCompletionSource<bool>();

        // Регистрируем отмену
        var ctr = cancellationToken.Register(() =>
        {
            dispatcher.InvokeAsync(() =>
            {
                FindDialogPanel(sender)?.Hide();
            });

            tcs.TrySetResult(false);
        });

        dispatcher.InvokeAsync(async () =>
        {
            try
            {
                var panel = FindDialogPanel(sender);

                if (panel is null)
                {
                    // Окна нет — показываем модально
                    var modalResult = await ShowModal(sender, dialog, cancellationToken);
                    tcs.TrySetResult(modalResult);
                    return;
                }

                dialog.Completed += b =>
                {
                    panel.Hide();
                    tcs.TrySetResult(b);
                };

                panel.Show(dialog, dialog.StaysOpen);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });

        return tcs.Task;
    }


    /// <summary>
    /// Показать модальный диалог в потоке UI
    /// </summary>
    /// <param name="sender">Может быть Null. Объект, в котором будет найдена панель и установлен Owner модального окна. Если не найдена - будет окно поверх всех окон</param>
    /// <param name="dialog">Диалог для показа</param>
    /// <param name="cancellationToken">Токен отмены. При вызове CancellationRequested будет возвращено false</param>
    internal static Task<bool> ShowModal(DependencyObject sender, IWPRDialog dialog, CancellationToken cancellationToken = default)
    {
        var dispatcher = sender?.Dispatcher ?? Application.Current.Dispatcher;

        if (dispatcher == null || dispatcher.HasShutdownStarted || cancellationToken.IsCancellationRequested)
            return Task.FromResult(false);

        var tcs = new TaskCompletionSource<bool>();

        dispatcher.InvokeAsync(() =>
        {
            try
            {
                var owner = sender is null ? null : sender as Window ?? sender.FindVisualParent<Window>();
                var panel = FindDialogPanel(owner);

                var style = _ModalWindowStyle;
                var dlg = new Window
                {
                    WindowStartupLocation = panel is null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner,
                    Owner = owner,
                    Topmost = owner is null,
                    Content = dialog.DialogContent,
                    
                };
                if(style is not null)
                    dlg.Style = style;

                dialog.Completed += b =>
                {
                    if (!tcs.Task.IsCompleted)
                    {
                        tcs.TrySetResult(b);
                        dlg.Close();
                    }
                };

                using var ctr = cancellationToken.Register(() =>
                {
                    dispatcher.InvokeAsync(() =>
                    {
                        if (!tcs.Task.IsCompleted)
                        {
                            dlg.Close();        // Закрываем окно
                            panel?.Hide();      // Прячем панель
                            tcs.TrySetResult(false);
                        }
                    });
                });

                panel?.Show(null, true);

                dlg.ShowDialog();

                panel?.Hide();

                // Если Completed не вызвался и токен не отменился
                if (!tcs.Task.IsCompleted)
                    tcs.TrySetResult(false);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });

        return tcs.Task;
    }



    /// <summary>
    /// Показать всплывающее уведомление
    /// </summary>
    /// <param name="sender">Может быть Null. Объект, в котором будет найдена панель для показа уведомления. Если не найдена - будет модальное окно</param>
    /// <param name="message">Текст уведомления</param>
    /// <param name="background">Цвет фона</param>
    /// <param name="duration">Длительность показа</param>
    /// <param name="actionButtonText">Если задано - будет ожидать клика, при клике вернёт true. Если не задано - задача вернёт true сразу</param>
    internal static Task<bool> ShowNotification(
        DependencyObject sender,
        string message,
        StyleBrushes background,
        int duration,
        string actionButtonText = null)
    {
        var dispatcher = sender?.Dispatcher ?? Application.Current.Dispatcher;

        if (dispatcher == null || dispatcher.HasShutdownStarted)
            return Task.FromResult(false);

        var tcs = new TaskCompletionSource<bool>();

        dispatcher.InvokeAsync(() =>
        {
            try
            {
                var panel = FindDialogPanel(sender);

                if (panel == null)
                {
                    // Если панели нет — показываем как модальное окно
                    var dlg = new MessageDialog
                    {
                        Content = message,
                        DialogType = DialogType.Information,
                        IsErrorMessage = background == StyleBrushes.DangerColorBrush
                    };

                    // Ждём ShowModal, но внутри InvokeAsync нельзя await, значит:
                    _ = ShowModal(sender, dlg).ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                            tcs.TrySetResult(task.Result);
                        else if (task.IsFaulted)
                            tcs.TrySetException(task.Exception!);
                        else
                            tcs.TrySetResult(false);
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                    return;
                }

                if (actionButtonText == null)
                {
                    panel.ShowNotification(message, duration, null, null, background);
                    tcs.TrySetResult(true);
                }
                else
                {
                    panel.ShowNotification(message, duration, actionButtonText, b => tcs.TrySetResult(b), background);
                }
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });

        return tcs.Task;
    }


    #endregion


    /// <summary>
    /// Диалог по умолчанию. Показывает уведомления в активном окне.
    /// </summary>
    public static IUserDialog Default { get; } = new WPRUserDialog();

    /// <summary>
    /// Модальный диалог по умолчанию
    /// </summary>
    public static IUserDialog ModalDialog { get; } = new WPRUserDialog { IsModal = true };


    /// <summary>
    /// Получить экземпляр диалога для привязок к конкретным панелям в разметке
    /// </summary>
    public static IUserDialog CreateUserDialog() => new WPRUserDialog();
}