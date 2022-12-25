using System;

namespace WPR.Dialogs.Base;

/// <summary> Базовая реализация диалога </summary>
public class WPRDialog : IWPRDialog
{

    public WPRDialog(object Content, Action<bool> OnSetResult, bool StaysOpen = true)
    {
        DialogContent = Content;
        SetDialogResult += OnSetResult;
        this.StaysOpen = StaysOpen;
    }

    public Action<bool> SetDialogResult { get; set; }

    public object DialogContent { get; }

    public bool StaysOpen { get; }
}