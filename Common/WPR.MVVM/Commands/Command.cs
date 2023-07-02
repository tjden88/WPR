namespace WPR.MVVM.Commands;

/// <summary>
/// Реализация базовой команды
/// </summary>
public class Command : BaseCommand
{
    private readonly Action<object> _Execute;
    private readonly Predicate<object> _CanExecute;



    public Command(Action Execute, string CommandText = null)
        : this(Execute, null, CommandText)
    {
    }

    public Command(Action Execute, Func<bool> CanExecute, string CommandText = null)
        : this(P => Execute(), CanExecute is null ? null : _ => CanExecute(), CommandText)
    {
    }

    public Command(Action<object> Execute, Predicate<object> CanExecute = null, string CommandText = null)
    {
        _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
        _CanExecute = CanExecute;
        Text = CommandText;
    }


    /// <summary>Возможность выполнения команды</summary>
    public override bool CanExecute(object P) => _CanExecute?.Invoke(P) ?? true;

    /// <summary>Выполнить команду</summary>
    public override void Execute(object P) => _Execute(P);
}