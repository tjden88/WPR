namespace WPR.Abstractions.Models.Dialogs;

/// <summary>
/// Настройка валидации и свойств диалога ввода пользователя
/// </summary>
public class InputDialogFilterOptions
{
    /// <summary> Значение по умолчанию </summary>
    public string? DefaultValue { get; set; }

    /// <summary> Заголовок </summary>
    public string Title { get; set; } = "Ввод текста";

    /// <summary> Дополнительное сообщение </summary>
    public string? Message { get; set; }

    /// <summary> Многострочный ввод </summary>
    public bool MultiLine { get; set; } = false;

    /// <summary> Правила валидации </summary>
    public IList<ValidationRule> ValidationRules { get; set; } = [];


}