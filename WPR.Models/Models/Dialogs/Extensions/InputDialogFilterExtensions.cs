namespace WPR.Domain.Models.Dialogs.Extensions;

/// <summary> Методы расширения для фильтра пользовательского диалога </summary>
public static class InputDialogFilterExtensions
{
    /// <summary> Значение обязательно </summary>
    public static InputDialogFilter AddRequired(this InputDialogFilter filter, string ErrorMessage = "Значение обязательно") =>
        filter.AddRule(s => !string.IsNullOrWhiteSpace(s), ErrorMessage);


    /// <summary> Фильтр минимальной длины </summary>
    public static InputDialogFilter AddMinLen(this InputDialogFilter filter, int minLenght) =>
        filter.AddRule(s => s?.Length >= minLenght, $"Минимальная длина - {minLenght} символа (ов)");


    /// <summary> Фильтр максимальной длины </summary>
    public static InputDialogFilter AddMaxLen(this InputDialogFilter filter, int maxLenght) =>
        filter.AddRule(s => s?.Length <= maxLenght, $"Максимальная длина - {maxLenght} символа (ов)");


    /// <summary> Значение не должно содержать элементы данной последовательности </summary>
    public static InputDialogFilter AddMustNotContains(this InputDialogFilter filter, IEnumerable<string> values, string ErrorMessage = "Это значение уже существует") =>
        filter.AddRule(s => s is null || values.All(v => !string.Equals(v, s, StringComparison.OrdinalIgnoreCase)), ErrorMessage);



    /// <summary> Добавить правило валидации </summary>
    public static InputDialogFilter AddRule(this InputDialogFilter filter, Predicate<string?> Rule, string ErrorMessage)
    {
        filter.ValidationRules.Add(new InputDialogFilter.ValidationRule(Rule, ErrorMessage));
        return filter;
    }


    /// <summary> Установить дополнительное сообщение </summary>
    public static InputDialogFilter AddMessage(this InputDialogFilter filter, string Message)
    {
        filter.Message = Message;
        return filter;
    }


    /// <summary> Установить значение по умолчанию </summary>
    public static InputDialogFilter AddDefaultValue(this InputDialogFilter filter, string DefaultValue)
    {
        filter.DefaultValue = DefaultValue;
        return filter;
    }

}