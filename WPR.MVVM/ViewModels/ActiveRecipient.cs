using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Базовая модель-представление для MVVM.
/// Также реализует <see cref="ObservableRecipient"/> для поддержки сообщений. Сразу активна (готова к приёму сообщений).
/// </summary>
public abstract partial class ActiveRecipient : InactiveRecipient
{

    protected ActiveRecipient() : this(WeakReferenceMessenger.Default)
    {
    }

    protected ActiveRecipient(IMessenger messenger) : base(messenger)
    {
        IsActive = true;
    }
}