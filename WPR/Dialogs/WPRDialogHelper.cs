using System.Diagnostics.CodeAnalysis;
using System.Windows;
using WPR.Infrastructure.Extensions;

namespace WPR.Dialogs;

/// <summary>Диалоговые окна</summary>
internal static class UserDialogHelper
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


    private static Task<IWPRDialog> Show(DependencyObject sender, IWPRDialog dialog)
    {
        // Ищем панель
        var panel = FindDialogPanel(sender);

        if (panel is null)
        {
           // Modal
            return Task.FromResult(dialog);
        }

        var complete = new TaskCompletionSource<IWPRDialog>();

        dialog.Completed += b =>
        {
            panel.Hide();
            complete.TrySetResult(dialog);
        };
        panel.Show(dialog, dialog.StaysOpen);

        return complete.Task;
    }


}

public static class UserDialog
{

    /// <summary>
    /// Экземпляр диалога, который будет использоваться в приложении.
    /// </summary>
    public static IUserDialog Default { get; } = new WPRUserDialog();
}