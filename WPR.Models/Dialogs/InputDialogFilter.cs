namespace WPR.Models.Dialogs;

/// <summary>
/// Настройка валидации и свойств диалога ввода пользователя
/// </summary>
public class InputDialogFilter
{
    public InputDialogFilter(string Title)
    {
        this.Title = Title;
    }


    /// <summary> Значение по умолчанию </summary>
    public string? DefaultValue { get; set; }

    /// <summary> Заголовок </summary>
    public string Title { get; init; }

    /// <summary> Дополнительное сообщение </summary>
    public string? Message { get; set; }

    /// <summary> Правила валидации </summary>
    public IList<ValidationRule> ValidationRules { get; set; } = new List<ValidationRule>();



    /// <summary> Правила валидации </summary>
    public record ValidationRule(Predicate<string?> Rule, string ErrorMessage);


    #region FluentFactory

   

    


    #endregion
}