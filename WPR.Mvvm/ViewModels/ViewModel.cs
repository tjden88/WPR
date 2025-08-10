using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using System.Reflection;

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



public abstract class ViewModel<T>(T Model) : ObservableObject where T : class
{
    protected readonly T Model = Model ?? throw new ArgumentNullException(nameof(Model));

    // преобразование VM -> Model
    public static implicit operator T (ViewModel<T> vm)
    {
        return vm.Model;
    }
}