using System.Runtime.CompilerServices;

namespace WPR.MVVM.ViewModels;

/// <summary>
/// Базовая модель для редактирования сущности.
/// Может запоминать изначальные значения свойств и сбрасывать их.
/// Проверяет валидацию
/// </summary>
public abstract class EditEntityViewModel : ValidationViewModel
{

    // словарь для хранения изменённых свойств
    protected readonly Dictionary<string, object> Properties = new();


    /// <summary> Получить свойство вьюмодели </summary>
    protected virtual T Get<T>([CallerMemberName] string Property = null)
    {
        if (Property is null) throw new ArgumentNullException(nameof(Property));

        return (T)(Properties
            .TryGetValue(Property, out var value)
            ? value
            : default(T));
    }


    /// <summary> Установить свойство вьюмодели </summary>
    protected virtual bool Set<T>(T Value, [CallerMemberName] string Property = null)
    {
        if (Property is null) throw new ArgumentNullException(nameof(Property));

        if (Properties.TryGetValue(Property, out var oldValue) && Equals(oldValue, Value))
            return false;

        Properties[Property] = Value;

        OnPropertyChanged(Property);
        OnPropertyChanged(nameof(HasChanges));
        return true;
    }

    /// <summary> Имеются ли изменённые свойства модели </summary>
    public virtual bool HasChanges => Properties.Any();


    /// <summary> Зафиксировать изменения и очистить свойства </summary>
    public virtual void CommitChanges()
    {
        OnCommitChanges();

        Properties.Clear();
        OnPropertyChanged(nameof(HasChanges));
    }


    /// <summary> Вызывается при фиксации изменений, перед очисткой словаря изменённых свойств </summary>
    protected abstract void OnCommitChanges();



    /// <summary> Сбросить изменения </summary>
    public virtual void RejectChanges()
    {
        var changedProperties = Properties.Keys.ToArray();
        Properties.Clear();
        OnPropertyChanged(nameof(HasChanges));

        foreach (var property in changedProperties)
            OnPropertyChanged(property);
    }
}


/// <summary>
/// Типизированная модель для редактирования сущности.
/// Может запоминать изначальные значения свойств и сбрасывать их.
/// Проверяет валидацию
/// </summary>
/// <typeparam name="TModel">Тип редактируемой сущности</typeparam>
public abstract class EditEntityViewModel<TModel> : EditEntityViewModel
{
    protected TModel Model { get; }
    private readonly Type _ModelType;

    protected EditEntityViewModel(TModel Model)
    {
        this.Model = Model;
        _ModelType = typeof(TModel);
    }

    

    /// <summary> Получить свойство вьюмодели </summary>
    protected override T Get<T>([CallerMemberName] string Property = null)
    {
        if (Property is null) throw new ArgumentNullException(nameof(Property));

        return (T)(Properties
            .TryGetValue(Property, out var value)
            ? value
            : _ModelType.GetProperty(Property)?.GetValue(Model));
    }


    /// <summary> Установить свойство вьюмодели </summary>
    protected override bool Set<T>(T Value, [CallerMemberName] string Property = null)
    {
        if (Property is null) throw new ArgumentNullException(nameof(Property));

        if (Properties.TryGetValue(Property, out var oldValue) && Equals(oldValue, Value))
            return false;

        var defaultValue = _ModelType.GetProperty(Property)?.GetValue(Model);

        if (Equals(defaultValue, Value))
            Properties.Remove(Property);
        else
            Properties[Property] = Value;

        OnPropertyChanged(Property);
        OnPropertyChanged(nameof(HasChanges));
        return true;
    }

    protected override void OnCommitChanges()
    {
        foreach (var property in Properties)
        {
            var modelProperty = _ModelType.GetProperty(property.Key);
            if (modelProperty is null || !modelProperty.CanWrite) continue;

            modelProperty.SetValue(Model, property.Value);
        }
    }
}
