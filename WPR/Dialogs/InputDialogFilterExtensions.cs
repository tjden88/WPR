namespace WPR.Dialogs;

/// <summary> Методы расширения для фильтра пользовательского диалога </summary>
public static class InputDialogFilterExtensions
{
    /// <summary> Значение обязательно </summary>
    public static InputDialogFilterOptions AddRequired(this InputDialogFilterOptions filter, string ErrorMessage = "Значение обязательно") =>
        filter.AddRule(s => !string.IsNullOrWhiteSpace(s), ErrorMessage);


    /// <summary> Фильтр минимальной длины </summary>
    public static InputDialogFilterOptions AddMinLen(this InputDialogFilterOptions filter, int minLenght) =>
        filter.AddRule(s => s?.Length >= minLenght, $"Минимальная длина - {minLenght} символа (ов)");


    /// <summary> Фильтр максимальной длины </summary>
    public static InputDialogFilterOptions AddMaxLen(this InputDialogFilterOptions filter, int maxLenght) =>
        filter.AddRule(s => s?.Length <= maxLenght, $"Максимальная длина - {maxLenght} символа (ов)");


    /// <summary> Значение не должно содержать элементы данной последовательности </summary>
    public static InputDialogFilterOptions AddMustNotContains(this InputDialogFilterOptions filter, IEnumerable<string> values, string ErrorMessage = "Это значение уже существует") =>
        filter.AddRule(s => s is null || values.All(v => !string.Equals(v, s, StringComparison.OrdinalIgnoreCase)), ErrorMessage);



    /// <summary> Добавить правило валидации </summary>
    public static InputDialogFilterOptions AddRule(this InputDialogFilterOptions filter, Predicate<string> Rule, string ErrorMessage)
    {
        filter.Validation.Add(new ValidationInfo(Rule, ErrorMessage));
        return filter;
    }


    /// <summary> Установить заголовок </summary>
    public static InputDialogFilterOptions AddTitle(this InputDialogFilterOptions filter, string Title)
    {
        filter.Title = Title;
        return filter;
    }

    /// <summary> Установить дополнительное сообщение </summary>
    public static InputDialogFilterOptions AddMessage(this InputDialogFilterOptions filter, string Message)
    {
        filter.Message = Message;
        return filter;
    }


    /// <summary> Установить значение по умолчанию </summary>
    public static InputDialogFilterOptions AddDefaultValue(this InputDialogFilterOptions filter, string DefaultValue)
    {
        filter.DefaultValue = DefaultValue;
        return filter;
    }

    /// <summary> Разрешить многострочный ввод </summary>
    public static InputDialogFilterOptions SetMultiline(this InputDialogFilterOptions filter, bool MultiLine = true)
    {
        filter.MultiLine = MultiLine;
        return filter;
    }

}