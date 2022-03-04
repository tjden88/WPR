using System;

namespace WPR;

/// <summary> Интерфейс для реализации объекта в качестве диалогового окна </summary>
public interface IWPRDialog
{
    /// <summary>Результат диалога</summary>
    Action<bool> DialogResult { get; set; }

    /// <summary>Контент диалога</summary>
    object DialogContent { get; set; }

    /// <summary> Оставлять диалог открытым при клике вне его области </summary>
    bool StaysOpen { get; set; }
}