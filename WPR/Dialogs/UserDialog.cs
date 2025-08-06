using System.Diagnostics.CodeAnalysis;
using System.Windows;
using WPR.Infrastructure.Extensions;

namespace WPR.Dialogs;
public static class UserDialog
{
    #region Internal

    private static readonly Style _ModalWindowStyle = (Style)Application.Current.Resources["WPRModalWindow"];

    // Найти панель для отображения диалога
    [return: MaybeNull]
    private static WPRDialogPanel FindDialogPanel(DependencyObject uIElement)
    {
        if (uIElement == null)
            return null;

        if (uIElement is Window window)
            return window.Template?.FindName("WindowDialogPanel", window) as WPRDialogPanel;

        return uIElement.FindVisualParent<WPRDialogPanel>();
    }


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

                var dlg = new Window
                {
                    WindowStartupLocation = panel is null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner,
                    Owner = owner,
                    Topmost = owner is null,
                    Content = dialog.DialogContent,
                    Style = _ModalWindowStyle
                };

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


    #endregion


    /// <summary>
    /// Диалог по умолчанию. Показывает уведомления в активном окне.
    /// </summary>
    public static IUserDialog Default { get; } = new WPRUserDialog();

    /// <summary>
    /// Модальный диалог по умолчанию
    /// </summary>
    public static IUserDialog ModalDialog { get; } = new WPRUserDialog {IsModal = true};


    /// <summary>
    /// Получить экземпляр диалога для привязок к конкретным панелям в разметке
    /// </summary>
    public static IUserDialog GetUserDialog() => new WPRUserDialog();
}