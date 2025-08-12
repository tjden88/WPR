namespace WPR.Mvvm.Attributes;

/// <summary>
/// Автоматически уведомляет об изменении состояния команды с помощью CommandManager.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AutoNotifyCanExecuteChangedAttribute : Attribute
{
    public string? CommandName { get; set; }

    public AutoNotifyCanExecuteChangedAttribute() { }

    public AutoNotifyCanExecuteChangedAttribute(string CommandName)
    {
        this.CommandName = CommandName;
    }
}