namespace WPR.MVVM.Commands.Base;

/// <summary>
/// Типизированная базовая реализация команды
/// </summary>
/// <typeparam name="T">Тип параметра</typeparam>
public class Command<T> : BaseCommand<T>
{
    private readonly Action<T> _Execute;
    private readonly Predicate<T> _CanExecute;


    public Command(Action<T> Execute, Predicate<T> CanExecute = null, string CommandText = null)
    {
        _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
        _CanExecute = CanExecute;
        Text = CommandText;
    }


    /// <summary>Возможность выполнения команды</summary>
    public override bool CanExecute(object P)
    {

        if (!CanExecuteWithNullParameter && P is not T) return false;
        return _CanExecute?.Invoke((T)P) ?? true;
    }

    /// <summary>Выполнить команду</summary>
    public override void ExecuteCommand(T p) => _Execute(p);
}