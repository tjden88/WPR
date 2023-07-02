using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WPR.MVVM.Commands.Base;

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
            ExecutableChanged?.Invoke(this, value);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged();
        }
    }
    #endregion


    #region IsCanExecute : IsCanExecute - Можно ли выполнить команду

    private bool _IsCanExecute;

    /// <summary>Можно ли выполнить команду</summary>
    public bool IsCanExecute
    {
        get => _IsCanExecute;
        set
        {
            if (Equals(_IsCanExecute, value)) return;
            _IsCanExecute = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged();
        }
    }

    #endregion


    #region Текст команды

    private string _CommandText;

    /// <summary> Текст - описание команды </summary>
    public string Text
    {
        get => _CommandText;
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


    #region ICommand

    public event EventHandler CanExecuteChanged;

    /// <summary>Возможность выполнения команды</summary>
    bool ICommand.CanExecute(object parameter)
    {
        var canExecute = _Executable && CanExecute(parameter);
        IsCanExecute = canExecute;
        return canExecute;
    }

    /// <summary>Выполнить команду с параметром</summary>
    void ICommand.Execute(object parameter)
    {
        if (!((ICommand)this).CanExecute(parameter)) return;
        Execute(parameter);
    }

    #endregion

    ///// <summary> Выполнить команду без параметра </summary>
    public virtual void Execute() => Execute(null);

    /// <summary>Возможность выполнения команды</summary>
    public virtual bool CanExecute(object p) => ((ICommand)this).CanExecute(p);

    /// <summary>Действие выполнения команды</summary>
    public abstract void Execute(object p);

    public override string ToString() => Text;

    /// <summary> Объявить об изменении возможности выполнения команды </summary>
    protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

}