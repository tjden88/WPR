using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WPR.MVVM.ViewModels
{
    public abstract partial class ViewModel : INotifyPropertyChanged
    {
        /// <summary>Признак того, что мы находимся в режиме разработки под Visual Studio</summary>
        public static bool IsDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

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
            var oldValue = field;
            var res = Set(ref field, value, PropertyName);
            return new ValueResult<T>(res, value, oldValue, this);
        }

        protected ValueResult<T> IfSetRef<T>(ref T field, ref T value, [CallerMemberName] string PropertyName = null)
        {
            var oldValue = field;
            var res = SetRef(ref field, ref value, PropertyName);
            return new ValueResult<T>(res, value, oldValue, this);
        }


        /// <summary> Обновить все свойства ViewModel </summary>
        public virtual void OnAllPropertiesChanged()
        {
            foreach (var propertyInfo in GetType().GetProperties()) 
                OnPropertyChanged(propertyInfo.Name);
        }
    }
}
