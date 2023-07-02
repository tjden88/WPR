namespace WPR.MVVM.Commands;

/// <summary>
/// Базовая асинхронная реализация команды.
/// </summary>
public class AsyncCommand : BaseCommand
{
    private readonly Func<object, CancellationToken, Task> _ExecuteAsync;
    private readonly Predicate<object> _CanExecute;

    #region IsNowExecuting

    private bool _IsNowExecuting;

    /// <summary> Выполняется ли команда в текущий момент </summary>
    public bool IsNowExecuting
    {
        get => _IsNowExecuting;
        protected set
        {
            _IsNowExecuting = value;
            RaiseCanExecuteChanged();
            OnPropertyChanged();
        }
    }

    #endregion

    #region Cancel

    private CancellationTokenSource _CancelSource;

    /// <summary>
    /// Токен отмены операции.
    /// При завершении команды свойство очищается.
    /// Если не передан извне, создаётся новый для каждой операции выполнения команды.
    /// </summary>
    public CancellationTokenSource CancelSource
    {
        get => _CancelSource;
        set
        {
            _CancelSource?.Dispose();

            _CancelSource = value;
            OnPropertyChanged();
        }
    }

    /// <summary> Отмена выполнения команды </summary>
    public void CancelExecute() => CancelSource?.Cancel();

    #endregion

    #region Ctor

    protected AsyncCommand()
    {
    }

    public AsyncCommand(Func<CancellationToken, Task> ExecuteAsync, Predicate<object> CanExecute = null, string CommandText = null) :
        this((_, t) => ExecuteAsync(t), CanExecute, CommandText)
    {
    }

    public AsyncCommand(Func<CancellationToken, Task> ExecuteAsync, Func<bool> CanExecute = null, string CommandText = null) :
        this((_, t) => ExecuteAsync(t), CanExecute is null ? null : _ => CanExecute(), CommandText)
    {
    }

    public AsyncCommand(Func<object, CancellationToken, Task> ExecuteAsync, Predicate<object> CanExecute = null, string CommandText = null)
    {
        _ExecuteAsync = ExecuteAsync;
        _CanExecute = CanExecute;
        Text = CommandText;
    }

    #endregion

    public override bool CanExecute(object P) => !IsNowExecuting && (_CanExecute?.Invoke(P) ?? true);

    public override async void Execute(object P)
    {
        var cancelSourceisNull = _CancelSource == null;
        try
        {
            if (cancelSourceisNull) CancelSource = new();

            IsNowExecuting = true;

            await _ExecuteAsync(P, CancelSource.Token).ConfigureAwait(true);
        }
        finally
        {
            IsNowExecuting = false;
            if (cancelSourceisNull)
            {
                CancelSource?.Dispose();
                CancelSource = null;
            }
        }
    }
}