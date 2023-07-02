using System.Windows;
using System.Windows.Input;

namespace WPR.MVVM.Commands.Base;

/// <summary>
/// Типизированная асинхронная команда
/// </summary>
/// <typeparam name="T"></typeparam>
public class AsyncCommand<T> : AsyncCommand
{

    private readonly Func<T, CancellationToken, Task> _ExecuteAsync;
    private readonly Predicate<T> _CanExecute;

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
            CommandManager.InvalidateRequerySuggested();
            OnPropertyChanged();
        }
    }

    #endregion

    #region Ctor

    public AsyncCommand(Func<T, Task> ExecuteAsync, Predicate<T> CanExecute = null, string CommandText = null) :
        this(ExecuteAsync, CanExecute, CommandText, null, null)
    {
    }

    public AsyncCommand(Func<T, CancellationToken, Task> ExecuteAsync, Predicate<T> CanExecute, string CommandText) :
        this(ExecuteAsync, CanExecute, CommandText, null, null)
    {
    }

    public AsyncCommand(Func<T, Task> ExecuteAsync, Predicate<T> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget) :
        this((o, _) => ExecuteAsync(o), CanExecute, CommandText, ExecuteGesture, GestureTarget)
    {
    }


    public AsyncCommand(Func<T, CancellationToken, Task> ExecuteAsync, Predicate<T> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget)
    {
        _ExecuteAsync = ExecuteAsync;
        _CanExecute = CanExecute;
        Text = CommandText;

        this.ExecuteGesture = ExecuteGesture;
        GestureTarget?.InputBindings.Add(new InputBinding(this, ExecuteGesture));
    }

    #endregion


    protected override bool CanExecute(object P)
    {
        if (!CanExecuteWithNullParameter && P is not T) return false;
        return _CanExecute?.Invoke((T)P) ?? true;
    }

    protected override async void Execute(object P)
    {
        try
        {
            CancelSource ??= new();

            IsNowExecuting = true;

            await _ExecuteAsync((T)P, CancelSource.Token).ConfigureAwait(true);
        }
        catch (OperationCanceledException) { }
        finally
        {
            IsNowExecuting = false;
            CancelSource = null;
        }
    }
}