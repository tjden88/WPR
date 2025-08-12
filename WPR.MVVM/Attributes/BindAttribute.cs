namespace WPR.Mvvm.Attributes;

/// <summary>
/// Привязать свойство EditViewModel к свойству редактируемой модели.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class BindAttribute : Attribute
{
    public string? TargetPropertyName { get; }

    public BindAttribute() { }

    public BindAttribute(string targetPropertyName)
    {
        TargetPropertyName = targetPropertyName;
    }
}