namespace WPR.Mvvm.Attributes;

/// <summary>
/// Указывает, что при изменении этого свойства не нужно генерировать событие AfterPropertyChanged
/// </summary>

[AttributeUsage(AttributeTargets.Property)]
public class SkipAfterPropertyChangedAttribute : Attribute;