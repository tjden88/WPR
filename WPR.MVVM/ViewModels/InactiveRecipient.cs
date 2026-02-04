using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Базовая модель-представление для MVVM.
/// Также реализует <see cref="ObservableRecipient"/> для поддержки сообщений. Неактивна при создании. Может отправлять сообщения
/// </summary>
public abstract partial class InactiveRecipient : ObservableRecipient
{
    
    [JsonIgnore]
    protected new bool IsActive { get => base.IsActive; set => base.IsActive = value; }
    
    protected InactiveRecipient() : this(WeakReferenceMessenger.Default)
    {
    }
    
    protected InactiveRecipient(IMessenger messenger) : base(messenger)
    {
        this.InitializeAutoNotifyCanExecuteChangedAttribute();
    }
}