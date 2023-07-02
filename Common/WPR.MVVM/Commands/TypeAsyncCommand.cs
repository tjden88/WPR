namespace WPR.MVVM.Commands;

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
            RaiseCanExecuteChanged();
            OnPropertyChanged();
        }
    }

    #endregion

    #region Ctor


    public AsyncCommand(Func<T, CancellationToken, Task> ExecuteAsync, string CommandText = null) :
        this(ExecuteAsync, null, CommandText)
    {
    }


    public AsyncCommand(Func<T, CancellationToken, Task> ExecuteAsync, Predicate<T> CanExecute = null, string CommandText = null)
    {
        _ExecuteAsync = ExecuteAsync;
        _CanExecute = CanExecute;
        Text = CommandText;
    }

    #endregion


    public override bool CanExecute(object P)
    {
        if (!CanExecuteWithNullParameter && P is not T) return false;
        return _CanExecute?.Invoke((T)P) ?? true;
    }

    public override async void Execute(object P)
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