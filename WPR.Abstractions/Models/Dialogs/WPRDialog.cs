using Interfaces_IWPRDialog = WPR.Abstractions.Interfaces.IWPRDialog;

namespace WPR.Abstractions.Models.Dialogs;

/// <summary>
/// Базовая реализация диалога
/// Для вызова результат действия необходимо выполнить метод RaiseCompleted
/// </summary>
public class WPRDialog : Interfaces_IWPRDialog
{

    public WPRDialog(object Content, bool StaysOpen = true)
    {
        DialogContent = Content;
        this.StaysOpen = StaysOpen;
    }

    /// <summary> Установить результат диалога </summary>
    public void RaiseCompleted(bool result)
    {
        Completed?.Invoke(result);
    }

    public event Action<bool>? Completed;

    public object DialogContent { get; }

    public bool StaysOpen { get; }
}