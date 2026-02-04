using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using WPR.Mvvm.Attributes;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Модель-представление для редактирования сущности с возможностью отката изменений и автоматической привязкой свойств редактируемой модели с помощью атрибута BindAttribute.
/// </summary>
public abstract class EditViewModel<T> : ObservableValidator where T : class
{
    private readonly Dictionary<string, PropertyInfo> _BindProperties = new(); // Свойства модели-представления, которые будут привязаны к свойствам сущности с помощью атрибута BindAttribute
    private readonly Dictionary<string, object?> _OriginalValues = new(); // Словарь для хранения оригинальных значений свойств редактируемой сущности
    private readonly HashSet<string> _DirtyProperties = new(); // Список изменённых свойств, которые были изменены после загрузки модели

    protected T Model { get; }

    protected EditViewModel(T model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        PropertyChanged += OnViewModelPropertyChanged;
        InitializeBindings();
        LoadOriginalValues();
        this.InitializeAutoNotifyCanExecuteChangedAttribute();
    }

    private void InitializeBindings()
    {
        var members = GetType()
            .GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(m => m.MemberType is MemberTypes.Field or MemberTypes.Property);

        foreach (var member in members)
        {
            var attr = member
                .GetCustomAttributes(inherit: true)
                .OfType<BindAttribute>()
                .FirstOrDefault();

            if (attr == null) continue;

            string Capitalize(string s)
            {
                if (string.IsNullOrEmpty(s)) return s;
                return char.ToUpperInvariant(s[0]) + s[1..];
            }

            var vmPropName = member switch
            {
                FieldInfo f => Capitalize(f.Name.TrimStart('_')),
                PropertyInfo p => p.Name,
                _ => throw new InvalidOperationException("Неподдерживаемый тип члена")
            };

            var modelPropName = attr.TargetPropertyName ?? vmPropName;

            var modelProp = typeof(T).GetProperty(modelPropName);
            if (modelProp == null)
                throw new InvalidOperationException($"Модель не содержит свойства '{modelPropName}'");

            if (!modelProp.CanRead || !modelProp.CanWrite)
                throw new InvalidOperationException($"Свойство модели '{modelPropName}' должно быть доступно для чтения и записи");

            _BindProperties[vmPropName] = modelProp;
        }
    }


    // Загрузка оригинальных значений свойств из модели
    private void LoadOriginalValues()
    {
        _OriginalValues.Clear();

        foreach (var (vmPropName, modelProp) in _BindProperties)
        {
            var vmProp = GetType().GetProperty(vmPropName);
            if (vmProp == null)
                throw new InvalidOperationException($"Свойство ViewModel '{vmPropName}' не найдено.");

            var value = modelProp.GetValue(Model);
            vmProp.SetValue(this, value);

            _OriginalValues[vmPropName] = value;
            _DirtyProperties.Clear();
        }
    }

    // Отслеживание изменений привязанных свойств
    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName is null) return;
        if (!_BindProperties.ContainsKey(e.PropertyName)) return;

        var vmProp = GetType().GetProperty(e.PropertyName);
        if (vmProp == null) 
            throw new InvalidOperationException($"Свойство ViewModel '{e.PropertyName}' не найдено.");

        var currentValue = vmProp.GetValue(this);
        var originalValue = _OriginalValues.GetValueOrDefault(e.PropertyName);

        // Если текущее значение отличается от оригинального, добавляем в список изменённых свойств
        if (!Equals(currentValue, originalValue))
            _DirtyProperties.Add(e.PropertyName);
        else
            _DirtyProperties.Remove(e.PropertyName);

        // Уведомляем об изменении свойства HasChanges
        OnPropertyChanged(nameof(HasChanges));
    }


    #region Public

    /// <summary> Имеются ли изменённые свойства модели </summary>
    public virtual bool HasChanges => _DirtyProperties.Count > 0;


    /// <summary> Зафиксировать изменения в модели </summary>
    public virtual void CommitChanges()
    {
        foreach (var (vmPropName, modelProp) in _BindProperties)
        {
            var vmProp = GetType().GetProperty(vmPropName);
            if (vmProp == null) 
                throw new InvalidOperationException($"Свойство ViewModel '{vmPropName}' не найдено.");

            var value = vmProp.GetValue(this);
            modelProp.SetValue(Model, value);
        }

        LoadOriginalValues();
        OnPropertyChanged(nameof(HasChanges));
    }



    /// <summary> Сбросить изменения </summary>
    public virtual void ResetChanges()
    {
        LoadOriginalValues();
        OnPropertyChanged(nameof(HasChanges));
    }


    /// <summary>
    /// Обновить свойства модели-представления из модели (модель изменяется извне).
    /// </summary>
    public virtual void RefreshFromModel()
    {
        foreach (var (vmPropName, modelProp) in _BindProperties)
        {
            var vmProp = GetType().GetProperty(vmPropName);
            if (vmProp == null)
                throw new InvalidOperationException($"Свойство ViewModel '{vmPropName}' не найдено.");

            var value = modelProp.GetValue(Model);
            vmProp.SetValue(this, value);
        }

        // не перезаписываем оригинальные значения
    }


    /// <summary>
    /// Получить изменённые свойства и их оригинальные значения.
    /// </summary>
    /// <returns></returns>
    public virtual Dictionary<string, (object? Original, object? Current)> GetChanges() =>
        _DirtyProperties.ToDictionary(
            prop => prop,
            prop =>
            {
                var vmProp = GetType().GetProperty(prop);
                return (
                    Original: _OriginalValues.GetValueOrDefault(prop),
                    Current: vmProp?.GetValue(this)
                );
            });

    #endregion
    
}