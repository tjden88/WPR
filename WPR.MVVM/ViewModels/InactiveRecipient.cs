using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Базовая модель-представление для MVVM.
/// Также реализует <see cref="ObservableRecipient"/> для поддержки сообщений. Неактивна при создании. Может отправлять сообщения
/// </summary>
[ObservableRecipient]
public abstract partial class InactiveRecipient : ViewModel
{
    
    protected InactiveRecipient(IMessenger messenger)
    {
        Messenger = messenger;
    }

    protected InactiveRecipient() : this(WeakReferenceMessenger.Default)
    {

    }
}