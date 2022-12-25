using System.Globalization;
using System.Windows.Controls;

namespace WPR.MVVM.Validation.Base;

/// <summary>Базовый класс валидаций</summary>
public abstract class ValidationBase : ValidationRule
{
    /// <summary>Описание ошибки</summary>
    public string Message { get; set; } = "Неверное значение";

    /// <summary>Прошла ли проверка валидности</summary>
    public bool IsValid { get; private set; } = true;

    /// <summary>Провести валидацию</summary>
    protected abstract bool Validated(object value, CultureInfo cultureInfo);

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        IsValid = Validated(value, cultureInfo);
        return IsValid ? ValidationResult.ValidResult : new ValidationResult(false, Message);
    }
}

public abstract class ValidationBase<T> : ValidationRule
{
    /// <summary>Описание ошибки</summary>
    public string Message { get; set; } = "Неверное значение";

    /// <summary>Прошла ли проверка валидности</summary>
    public bool IsValid { get; private set; } = true;

    /// <summary>Провести валидацию</summary>
    protected abstract bool Validated(T value, CultureInfo cultureInfo);

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        IsValid = Validated((T)value, cultureInfo);
        return IsValid ? ValidationResult.ValidResult : new ValidationResult(false, Message);
    }
}