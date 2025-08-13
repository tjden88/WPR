using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Представляет базовый класс для создания моделей представлений, которые охватывают определенный тип модели и предоставляют уведомления об изменении свойств.
/// Реализует ObservableRecipient - через WeakReferenceMessenger.Default. Сразу активен
/// </summary>
/// <typeparam name="T">Тип обёрнутой модели. Должен быть ссылочным типом.</typeparam>
[ObservableRecipient]
public abstract partial class ObservableRecipientModel<T> : ObservableModel<T> where T : class
{
    protected ObservableRecipientModel(T Model) : base(Model)
    {
        Messenger = WeakReferenceMessenger.Default;
        IsActive = true;
    }
}