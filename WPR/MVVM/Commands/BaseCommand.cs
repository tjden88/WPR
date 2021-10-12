using System;
using System.Globalization;
using System.Windows.Input;

namespace WPR.MVVM.Commands
{
    public abstract class BaseCommand : ICommand
    {
        #region Executable: bool
        private bool _Executable = true;

        /// <summary>Ручная возможность выполнения команды</summary>
        public bool Executable
        {
            get => _Executable;
            set
            {
                if (_Executable == value) return;
                _Executable = value;
                ExecutableChanged?.Invoke(this, value);
                CommandManager.InvalidateRequerySuggested();
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
                return gString== null ? _CommandText : $"{_CommandText} ({gString})";
            }
            set => _CommandText = value;
        }

        #endregion

        /// <summary> Комбинация клавиш быстрого доступа </summary>
        protected  KeyGesture ExecuteGesture { get; init; }

        /// <summary>Происходит при изменении ручной возможности исполнения команды</summary>
        public event EventHandler<bool> ExecutableChanged;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        bool ICommand.CanExecute(object parameter) => _Executable && CanExecute(parameter);

        void ICommand.Execute(object parameter)
        {
            if (!((ICommand)this).CanExecute(parameter)) return;
            Execute(parameter);
        }

        ///// <summary> Выполнить команду без параметра </summary>
        public virtual void Execute() => Execute(null);

        /// <summary>Возможность выполнения команды</summary>
        public virtual bool CanExecute(object p) => true;

        /// <summary>Выполнить команду с параметром</summary>
        public abstract void Execute(object p);

        public override string ToString() => Text;
    }

}
