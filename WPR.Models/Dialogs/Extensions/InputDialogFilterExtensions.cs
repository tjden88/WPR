namespace WPR.Models.Dialogs.Extensions;

/// <summary> Методы расширения для фильтра пользовательского диалога </summary>
public static class InputDialogFilterExtensions
{
    /// <summary> Значение обязательно </summary>
    public static InputDialogFilter Required(this InputDialogFilter filter, string ErrorMessage = "Значение обязательно") =>
        AddRule(filter, s => !string.IsNullOrWhiteSpace(s), ErrorMessage);


    /// <summary> Фильтр минимальной длины </summary>
    public static InputDialogFilter MinLen(this InputDialogFilter filter, int minLenght) =>
        AddRule(filter, s => s?.Length >= minLenght, $"Минимальная длина - {minLenght} символа (ов)");


    /// <summary> Фильтр максимальной длины </summary>
    public static InputDialogFilter MaxLen(this InputDialogFilter filter, int maxLenght) =>
        AddRule(filter, s => s?.Length <= maxLenght, $"Максимальная длина - {maxLenght} символа (ов)");


    /// <summary> Значение не должно содержать элементы данной последовательности </summary>
    public static InputDialogFilter MustNotContains(this InputDialogFilter filter, IEnumerable<string> values, string ErrorMessage = "Это значение уже существует") =>
        AddRule(filter, s => s is null || values.All(v => !string.Equals(v, s, StringComparison.OrdinalIgnoreCase)), ErrorMessage);



    /// <summary> Добавить правило валидации </summary>
    public static InputDialogFilter AddRule(this InputDialogFilter filter, Predicate<string?> Rule, string ErrorMessage)
    {
        filter.ValidationRules.Add(new InputDialogFilter.ValidationRule(Rule, ErrorMessage));
        return filter;
    }


    /// <summary> Установить дополнительное сообщение </summary>
    public static InputDialogFilter SetMessage(this InputDialogFilter filter, string Message)
    {
        filter.Message = Message;
        return filter;
    }


    /// <summary> Установить значение по умолчанию </summary>
    public static InputDialogFilter SetDefaultValue(this InputDialogFilter filter, string DefaultValue)
    {
        filter.DefaultValue = DefaultValue;
        return filter;
    }

}