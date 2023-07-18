using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;

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
    protected TModel Model { get; private set; }
    private readonly Type _ModelType;

    private readonly Dictionary<string, PropertyInfo> _IncludedProperties = new(); // Свойства с аттрибутом IncludedPropertyAttribute: имя собственного свойства, свойство модели

    protected EditEntityViewModel(TModel Model) : this()
    {
        this.Model = Model;
    }

    protected EditEntityViewModel()
    {
        _ModelType = typeof(TModel);
        foreach (var property in GetType().GetProperties())
        {
            var attr = property.GetCustomAttributes(typeof(IncludedPropertyAttribute), true);
            if (attr.Length > 0)
            {
                var attribute = (IncludedPropertyAttribute)attr[0];
                var modelPropertyName = attribute.ModelPropertyName ?? property.Name;
                _IncludedProperties.Add(property.Name, _ModelType.GetProperty(modelPropertyName));
            }
        }
    }

    protected override void OnPropertyChanged(string PropertyName = null)
    {
        if (_IncludedProperties.TryGetValue(PropertyName!, out var modelProperty))
        {
            var property = GetType().GetProperty(PropertyName);

            var defaultValue = modelProperty?.GetValue(Model);
            var currentValue = property!.GetValue(this);

            if (Equals(defaultValue, currentValue))
                Properties.Remove(modelProperty!.Name);
            else
                Properties[modelProperty!.Name] = currentValue;

            base.OnPropertyChanged(PropertyName);
            OnPropertyChanged(nameof(HasChanges));
            return;
        }
        base.OnPropertyChanged(PropertyName);
    }


    /// <summary>
    /// Установить модель и начать отслеживание свойств
    /// </summary>
    /// <param name="model">Экземпляр модели</param>
    public virtual void SetModel(TModel model)
    {
        Properties.Clear();
        Model = model;
        foreach (var (key, value) in _IncludedProperties)
        {
            var prop = GetType().GetProperty(key);
            if (prop is null || !prop.CanWrite) continue;
            prop.SetValue(this, value?.GetValue(model));
        }
    }



    /// <summary> Получить свойство вьюмодели </summary>
    protected override T Get<T>([CallerMemberName] string Property = null)
    {
        if (Model == null) return default;
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

        foreach (var (key, value) in _IncludedProperties)
        {
            var prop = GetType().GetProperty(key);
            value.SetValue(Model, prop?.GetValue(this));
        }
    }



    /// <summary> 
    /// Данное свойство будет отслеживаться и учитываться при вызове методов SetModel и CommitChanges
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    protected class IncludedPropertyAttribute : Attribute
    {
        public IncludedPropertyAttribute(string ModelPropertyName = null)
        {
            this.ModelPropertyName = ModelPropertyName;
        }

        /// <summary>
        /// Имя свойства модели
        /// </summary>
        [MaybeNull]
        public string ModelPropertyName { get; init; }
    }
}
