using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPR.MVVM.ViewModels;

public abstract partial class ViewModel : INotifyPropertyChanged
{

    /// <summary>Признак того, что мы находимся в режиме разработки под Visual Studio</summary>
    public static bool IsDesignMode => (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

    /// <summary>
    /// Создать класс базовой модели-представления
    /// </summary>
    /// <param name="OnlyForDesignTime">При попытке вызова не в режиме работы дизайнера VS выбросит исключение</param>
    protected ViewModel(bool OnlyForDesignTime = false)
    {
        if (OnlyForDesignTime && !IsDesignMode)
            throw new InvalidOperationException(
                "Этот конструктор предназначен для использования только в режиме разработки Visual Studio");
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    #endregion

    /// <summary> Установить значение свойства </summary>
    protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(PropertyName);
        return true;
    }

    /// <summary> Установить значение свойства с помощью действия в другой объект</summary>
    protected virtual bool SetTo<T>(T field, T value, Action<T> to, [CallerMemberName] string PropertyName = null)
    {
        if (Equals(value, field)) return false;
        to.Invoke(value);
        OnPropertyChanged(PropertyName);
        return true;
    }

    /// <summary> Установить значение свойства (проверка по ссылке)</summary>
    protected virtual bool SetRef<T>(ref T field, ref T value, [CallerMemberName] string PropertyName = null)
    {
        if (ReferenceEquals(field, value)) return false;
        field = value;
        OnPropertyChanged(PropertyName);
        return true;
    }

    protected ValueResult<T> IfSet<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
    {
        var res = Set(ref field, value, PropertyName);
        return new ValueResult<T>(res, value, field, this);
    }

    protected ValueResult<T> IfSetRef<T>(ref T field, ref T value, [CallerMemberName] string PropertyName = null)
    {
        var res = SetRef(ref field, ref value, PropertyName);
        return new ValueResult<T>(res, value, field, this);
    }


    /// <summary> Обновить все свойства ViewModel </summary>
    public virtual void OnAllPropertiesChanged()
    {
        foreach (var propertyInfo in GetType().GetProperties()) 
            OnPropertyChanged(propertyInfo.Name);
    }

    #region WatchChanges


    /// <summary> Было ли изменено хотя бы одно свойство после вызова StartWatchPropertyChanged() </summary>
    protected bool PropertyWasChanged { get; private set; }

    /// <summary>
    /// Начать отслеживать изменения свойств
    /// При изменении любого свойства PropertyWasChanged будет установлен
    /// </summary>
    protected void StartWatchPropertyChanged()
    {
        PropertyChanged -= OnPropertyChangedWhenWatching;
        PropertyWasChanged = false;
        PropertyChanged += OnPropertyChangedWhenWatching;
    }

    /// <summary>
    /// Завершить отслеживание изменения свойств
    /// PropertyWasChanged будет сброшен
    /// </summary>
    protected void StopWatchPropertyChanged()
    {
        PropertyWasChanged = false;
        PropertyChanged -= OnPropertyChangedWhenWatching;
    }


    private void OnPropertyChangedWhenWatching(object Sender, PropertyChangedEventArgs E)
    {
        var propName = E.PropertyName;
        if (propName == null) return;

        var prop = GetType().GetProperty(propName);
        if (prop == null) return;
        if(!prop.CanRead || !prop.CanWrite) return;

       var attr= prop.GetCustomAttributes(typeof(SkipWatchAttribute), true);
       if (attr.Length > 0) return;

        PropertyWasChanged = true;
        PropertyChanged -= OnPropertyChangedWhenWatching;
    }

    #endregion


    /// <summary>
    /// Указывает, что это свойство нужно игнорировать при отслеживании изменений
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    protected class SkipWatchAttribute : Attribute
    {
    }
}