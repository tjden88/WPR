using System.Collections.ObjectModel;
using System.Windows.Input;

namespace WPR.MVVM.Commands.Base;

/// <summary>
/// Последовательное выполнение нескольких команд
/// </summary>
public class MultiCommand : Collection<ICommand>, ICommand
{

    public bool CanExecute(object parameter)
    {
        foreach (var cmd in this)
            if (!cmd.CanExecute(parameter))
                return false;

        return true;
    }

    public event EventHandler CanExecuteChanged;

    protected virtual void OnCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Execute(object parameter)
    {
        foreach (var cmd in this)
            cmd.Execute(parameter);
    }
}