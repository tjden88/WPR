using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Базовая модель-представление для MVVM.
/// Также реализует интерфейс <see cref="ObservableRecipient"/> для поддержки сообщений. Сразу активна.
/// </summary>
public abstract partial class ViewModel : ObservableRecipient
{
    protected ViewModel(IMessenger messenger) : base(messenger)
    {
        IsActive = true;
        this.InitializeAttributes();
    }



    protected ViewModel() : this(WeakReferenceMessenger.Default)
    {
    }

    /// <summary>
    /// Заголовок модели-представления.
    /// </summary>
    [ObservableProperty] private string _Title = string.Empty;


    /// <summary>
    /// Индикатор того, что модель-представление занята выполнением операции.
    /// </summary>
    [ObservableProperty] private bool _IsBusy;
}


/// <summary>
/// Автоматически уведомляет об изменении состояния команды с помощью CommandManager.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AutoNotifyCanExecuteChangedAttribute : Attribute
{
    public string? CommandName { get; set; }

    public AutoNotifyCanExecuteChangedAttribute()
    {
        
    }

    public AutoNotifyCanExecuteChangedAttribute(string CommandName)
    {
        this.CommandName = CommandName;
    }
}