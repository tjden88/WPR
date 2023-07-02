using System.Windows;
using System.Windows.Input;

namespace WPR.MVVM.Commands.Base;

/// <summary>
/// Реализация базовой команды
/// </summary>
public class Command : BaseCommand
{
    private readonly Action<object> _Execute;
    private readonly Predicate<object> _CanExecute;

    public Command(Action<object> Execute, Predicate<object> CanExecute = null, string CommandText = null)
        : this(Execute, CanExecute, CommandText, null, null)
    {
    }

    public Command(Action Execute, Func<bool> CanExecute = null, string CommandText = null)
        : this(Execute, CanExecute , CommandText, null, null)
    {
    }

    public Command(Action Execute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget) 
        : this(Execute, null, CommandText, ExecuteGesture, GestureTarget)
    {
    }

    public Command(Action Execute, Func<bool> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget)
        : this(P => Execute(), CanExecute is null ? null : _ => CanExecute(), CommandText, ExecuteGesture, GestureTarget)
    {
    }

    public Command(Action<object> Execute, Predicate<object> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget)
    {
        _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
        _CanExecute = CanExecute;
        Text = CommandText;

        this.ExecuteGesture = ExecuteGesture;
        GestureTarget?.InputBindings.Add(new InputBinding(this, ExecuteGesture));
    }

    /// <summary>Возможность выполнения команды</summary>
    protected override bool CanExecute(object P) => _CanExecute?.Invoke(P) ?? true;

    /// <summary>Выполнить команду</summary>
    protected override void Execute(object P) => _Execute(P);
}