using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Базовая модель-представление для MVVM.
/// Также реализует интерфейс <see cref="ObservableRecipient"/> для поддержки сообщений. Сразу активна.
/// </summary>
public abstract partial class ViewModel : ObservableRecipient
{
    [JsonIgnore]
    public new bool IsActive { get => base.IsActive; set => base.IsActive = value; }
    
    protected ViewModel(IMessenger messenger) : base(messenger)
    {
        IsActive = true;
        this.InitializeAttributes();
    }

    protected ViewModel() : this(WeakReferenceMessenger.Default)
    {
    }

}