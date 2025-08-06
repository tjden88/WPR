using System.Diagnostics.CodeAnalysis;
using System.Windows;
using WPR.Infrastructure.Extensions;

namespace WPR.Dialogs;

/// <summary>Диалоговые окна</summary> // todo сделать internal
public static class UserDialogHelper
{
    private static readonly Style _ModalWindowStyle = (Style) Application.Current.Resources["WPRModalWindow"];

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


    public static Task<bool> Show(DependencyObject sender, IWPRDialog dialog, CancellationToken cancellationToken = default)
    {
        var dispatcher = sender?.Dispatcher ?? Application.Current.Dispatcher;

        if (dispatcher == null || dispatcher.HasShutdownStarted || cancellationToken.IsCancellationRequested)
            return Task.FromResult(false);

        var tcs = new TaskCompletionSource<bool>();

        cancellationToken.Register(() =>
        {
            dispatcher.InvokeAsync(() =>
            {
                FindDialogPanel(sender)?.Hide();
            });

            tcs.TrySetResult(false);
        });

        dispatcher.InvokeAsync(() =>
        {
            try
            {
                var panel = FindDialogPanel(sender);
                if (panel is null)
                {
                    tcs.TrySetResult(false);
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


    public static Task<bool> ShowModal(DependencyObject sender, IWPRDialog content, CancellationToken cancellationToken = default)
    {
        var dispatcher = sender?.Dispatcher ?? Application.Current.Dispatcher; 

        if (dispatcher == null || dispatcher.HasShutdownStarted || cancellationToken.IsCancellationRequested)
            return Task.FromResult(false);

        var tcs = new TaskCompletionSource<bool>();

        dispatcher.InvokeAsync(() =>
        {
            try
            {
                var owner = sender as Window ?? sender.FindVisualParent<Window>();
                var panel = FindDialogPanel(owner);

                var dlg = new Window
                {
                    WindowStartupLocation = panel is null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner,
                    Owner = owner,
                    Topmost = owner is null,
                    Content = content.DialogContent,
                    Style = _ModalWindowStyle
                };

                content.Completed += b =>
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


}

public static class UserDialog
{

    /// <summary>
    /// Экземпляр диалога, который будет использоваться в приложении.
    /// </summary>
    public static IUserDialog Default { get; } = new WPRUserDialog();
}