using System;
namespace WPR;

/// <summary> Интерфейс для реализации объекта в качестве диалогового окна </summary>
public interface IWPRDialog
{
    /// <summary>Установить результат диалога</summary>
    Action<bool> SetDialogResult { get; set; }

    /// <summary>Контент диалога</summary>
    object DialogContent => this;

    /// <summary> Оставлять диалог открытым при клике вне его области </summary>
    bool StaysOpen => true;
}