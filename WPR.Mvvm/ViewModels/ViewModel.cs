using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Базовая модель-представление для MVVM.
/// Также реализует интерфейс <see cref="ObservableRecipient"/> для поддержки сообщений. Сразу активна.
/// </summary>
public abstract partial class ViewModel : ObservableRecipient, IDisposable
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

    public void Dispose()
    {
        this.InitializeAttributes(true);

        Debug.WriteLine($"Уничтожение {GetType().Name}");
    }
}