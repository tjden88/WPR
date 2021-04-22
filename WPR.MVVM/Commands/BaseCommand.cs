using System;
using System.Windows.Input;

namespace WPR.MVVM.Commands
{
    public abstract class BaseCommand : ICommand
    {
        private bool _Executable = true;
        /// <summary>Ручная возможность выполнения команды</summary>
        protected bool Executable
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
            if (!CanExecute(parameter)) return;
            Execute(parameter);
        }

        protected virtual bool CanExecute(object p) => true;

        protected abstract void Execute(object p);
    }

}
