using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Базовая модель-представление для MVVM.
/// Также реализует <see cref="ObservableRecipient"/> для поддержки сообщений. Сразу активна.
/// </summary>
public abstract partial class ActiveRecipient : ObservableRecipient
{
    
    [JsonIgnore]
    protected new bool IsActive { get => base.IsActive; set => base.IsActive = value; }
    
    protected ActiveRecipient() : this(WeakReferenceMessenger.Default)
    {
    }
    
    protected ActiveRecipient(IMessenger messenger) : base(messenger)
    {
        IsActive = true;
        this.InitializeAttributes();
    }
}