using System;

namespace WPR.Dialogs.Base;

/// <summary> Базовая реализация диалога </summary>
public class WPRDialog : IWPRDialog
{

    public WPRDialog(object Content, bool StaysOpen = true)
    {
        DialogContent = Content;
        this.StaysOpen = StaysOpen;
    }

    /// <summary> Установить результат диалога </summary>
    public void SetResult(bool result)
    {
        SetDialogResult?.Invoke(result);
    }

    public Action<bool> SetDialogResult { get; set; }

    public object DialogContent { get; }

    public bool StaysOpen { get; }
}