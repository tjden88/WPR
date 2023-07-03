namespace WPR.Domain.Interfaces;

/// <summary> Интерфейс для реализации объекта в качестве диалогового окна </summary>
public interface IWPRDialog
{
    /// <summary>Установить результат диалога</summary>
    event Action<bool>? Completed;

    /// <summary>Контент диалога</summary>
    object DialogContent => this;

    /// <summary> Оставлять диалог открытым при клике вне его области </summary>
    bool StaysOpen => true;

}