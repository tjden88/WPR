using System;
using System.Windows.Input;

namespace WPR.MVVM.Commands
{
    public abstract class BaseCommand : ICommand
    {
        private bool _Executable = true;

        /// <summary>Ручная возможность выполнения команды</summary>
        public bool Executable
        {
            get => _Executable;
            set
            {
                if (_Executable == value) return;
                _Executable = value;
                ExecutableChanged?.Invoke(this, EventArgs.Empty);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>Происходит при изменении ручной возможности исполнения команды</summary>
        public event EventHandler ExecutableChanged;

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
        //public virtual void Execute() => Execute(null);

        /// <summary>Возможность выполнения команды</summary>
        public virtual bool CanExecute(object p) => true;

        /// <summary>Выполнить команду с параметром</summary>
        public abstract void Execute(object p);
    }

}
