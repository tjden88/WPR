using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WPR.MVVM.Commands
{
    public abstract class BaseCommand : ICommand, INotifyPropertyChanged
    {
        #region Executable: bool

        /// <summary>Происходит при изменении ручной возможности исполнения команды</summary>
        public event EventHandler<bool> ExecutableChanged;

        private bool _Executable = true;

        /// <summary>Ручная возможность выполнения команды</summary>
        public bool Executable
        {
            get => _Executable;
            set
            {
                if (_Executable == value) return;
                _Executable = value;
                CommandManager.InvalidateRequerySuggested();
                ExecutableChanged?.Invoke(this, value);
                OnPropertyChanged();
            }
        }
        #endregion

        #region Текст команды
        private string _CommandText;
        /// <summary> Текст - описание команды </summary>
        public string Text
        {
            get
            {
                var gString = ExecuteGesture?.GetDisplayStringForCulture(CultureInfo.CurrentCulture);
                return gString == null ? _CommandText : $"{_CommandText} ({gString})";
            }
            set
            {
                if (value == _CommandText) return;
                _CommandText = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        #endregion

        /// <summary> Комбинация клавиш быстрого доступа </summary>
        protected KeyGesture ExecuteGesture { get; init; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>Возможность выполнения команды</summary>
        public bool CanExecute(object parameter) => _Executable && CanExecuteCommand(parameter);

        /// <summary>Выполнить команду с параметром</summary>
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            ExecuteCommand(parameter);
        }

        ///// <summary> Выполнить команду без параметра </summary>
        public virtual void Execute() => Execute(null);

        /// <summary>Возможность выполнения команды</summary>
        protected virtual bool CanExecuteCommand(object p) => true;

        /// <summary>Действие выполнения команды</summary>
        protected abstract void ExecuteCommand(object p);

        public override string ToString() => Text;
    }

}
