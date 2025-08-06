using System.Windows;
using System.Windows.Input;

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

    public Command(Action<T> Execute, Predicate<T> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget) 
        : this(Execute, CanExecute, CommandText)
    {
        this.ExecuteGesture = ExecuteGesture;
        GestureTarget?.InputBindings.Add(new InputBinding(this, ExecuteGesture));
    }

    /// <summary>Возможность выполнения команды</summary>
    protected override bool CanExecuteCommand(object P)
    {

        if (!CanExecuteWithNullParameter && P is not T) return false;

        return _CanExecute?.Invoke((T)P) ?? true;
    }

    /// <summary>Выполнить команду</summary>
    public override void ExecuteCommand(T p) => _Execute(p);
}