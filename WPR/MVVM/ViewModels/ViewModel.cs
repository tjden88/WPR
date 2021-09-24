using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPR.MVVM.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }

        protected virtual void IfSet<T>(ref T field, T value, Action<T> ActionIfPropertyChanged, [CallerMemberName] string PropertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(PropertyName);
                ActionIfPropertyChanged(value);
            }
        }

        /// <summary> Обновить все свойства ViewModel </summary>
        public virtual void OnAllPropertiesChanged()
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                OnPropertyChanged(propertyInfo.Name);
            }
        }
    }
}
