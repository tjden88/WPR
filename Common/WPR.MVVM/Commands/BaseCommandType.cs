namespace WPR.MVVM.Commands;

public abstract class BaseCommand<T> : BaseCommand
{
    #region CanExecuteWithNullParameter

    private bool _CanExecuteWithNullParameter;

    /// <summary>Разрешить выполнение команды с параметром = null</summary>
    public bool CanExecuteWithNullParameter
    {
        get => _CanExecuteWithNullParameter;
        set
        {
            if (_CanExecuteWithNullParameter == value) return;
            _CanExecuteWithNullParameter = value;
            RaiseCanExecuteChanged();
            OnPropertyChanged();
        }
    }

    #endregion

    /// <summary>Возможность выполнения команды</summary>
    public override bool CanExecute(object P)
    {

        if (!CanExecuteWithNullParameter && P is not T) return false;
        return true;
    }

    public sealed override void Execute(object p) => ExecuteCommand((T)p);

    /// <summary>Выполнить команду с параметорм типа</summary>
    public abstract void ExecuteCommand(T p);
}