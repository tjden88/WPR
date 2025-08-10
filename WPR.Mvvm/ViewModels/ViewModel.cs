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



public abstract class ViewModel<T> : ViewModel where T : class
{
    protected T Model { get; }
    
    protected ViewModel(T model) : this(model, WeakReferenceMessenger.Default)
    {
    }
    
    protected ViewModel(T model, IMessenger messenger) : base(messenger)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        LoadFromModel(model);
    }
    // преобразование VM -> Model
    public static implicit operator T (ViewModel<T> vm)
    {
        vm.SaveToModel();
        return vm.Model;
    }

    /// <summary>
    /// Копирует значения из модели в сгенерированные свойства
    /// </summary>
    public void LoadFromModel(T model)
    {
        var vmProps = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var modelProps = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (var vmProp in vmProps)
        {
            // Только публичные сеттеры
            if (!vmProp.CanWrite || vmProp.SetMethod is null || !vmProp.SetMethod.IsPublic) continue;

            var modelProp = modelProps.FirstOrDefault(p =>
                p.Name == vmProp.Name &&
                p.PropertyType == vmProp.PropertyType &&
                p.CanRead &&
                p.GetMethod is not null &&
                p.GetMethod.IsPublic);

            if (modelProp is null) continue;

            var value = modelProp.GetValue(model);
            vmProp.SetValue(this, value);
        }
    }

    /// <summary>
    /// Копирует значения из ViewModel обратно в модель
    /// </summary>
    protected virtual void SaveToModel()
    {
        if (Model is null)
            throw new InvalidOperationException("Model не инициализирована. Вызовите конструктор с моделью или LoadFromModel.");

        var vmProps = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var modelProps = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (var modelProp in modelProps)
        {
            // Только публичные сеттеры
            if (!modelProp.CanWrite || modelProp.SetMethod is null || !modelProp.SetMethod.IsPublic) continue;

            var vmProp = vmProps.FirstOrDefault(p =>
                p.Name == modelProp.Name &&
                p.PropertyType == modelProp.PropertyType &&
                p.CanRead &&
                p.GetMethod is not null &&
                p.GetMethod.IsPublic);

            if (vmProp is null) continue;

            var value = vmProp.GetValue(this);
            modelProp.SetValue(Model, value);
        }
    }
}