using System.Windows.Input;

namespace WPR.Infrastructure.Commands;

internal class BaseCommand(Action<object> execute, Predicate<object> canExecute) : ICommand
{
    private readonly Action<object> _Execute = execute ?? throw new ArgumentNullException(nameof(execute));


    public BaseCommand(Action Execute, Func<bool> CanExecute = null)
        : this(_ => Execute(), CanExecute is null ? null : _ => CanExecute())
    {
    }


    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>Возможность выполнения команды</summary>
    public bool CanExecute(object parameter)
    {
        var canExecute = CanExecuteCommand(parameter);
        return canExecute;
    }

    /// <summary>Выполнить команду с параметром</summary>
    public void Execute(object parameter)
    {
        if (!CanExecute(parameter)) return;
        ExecuteCommand(parameter);
    }

    ///// <summary> Выполнить команду без параметра </summary>
    public virtual void Execute() => Execute(null);

    /// <summary>Возможность выполнения команды</summary>
    protected bool CanExecuteCommand(object P) => canExecute?.Invoke(P) ?? true;

    /// <summary>Выполнить команду</summary>
    protected void ExecuteCommand(object P) => _Execute(P);


}
