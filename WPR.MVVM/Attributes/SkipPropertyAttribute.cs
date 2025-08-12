namespace WPR.Mvvm.Attributes;

/// <summary>
/// Указывает, что свойства модели не нужно генерировать в целевой класс ObservableModel.
/// Пример:
/// [SkipProperty("Name", "Orders")] или [SkipProperty("Name"), SkipProperty("Orders")]
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class SkipPropertyAttribute : Attribute
{
    /// <summary>Имена исключаемых свойств</summary>
    public IReadOnlyList<string> PropertyNames { get; }

    /// <param name="propertyNames">Имена исключаемых свойств</param>
    public SkipPropertyAttribute(params string[] propertyNames)
    {
        PropertyNames = propertyNames.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
    }
}