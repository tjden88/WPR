namespace WPR.Mvvm.Attributes;

/// <summary>
/// Указывает, что при изменении этого свойства нужно генерировать событие AfterPropertyChanged
/// </summary>

[AttributeUsage(AttributeTargets.Property)]
public class RaiseAfterPropertyChangedAttribute : Attribute;