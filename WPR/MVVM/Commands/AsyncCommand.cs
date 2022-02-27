using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPR.MVVM.Commands
{
    /// <summary>
    /// Базовая асинхронная реализация команды.
    /// Формирует задачу и запускает её
    /// </summary>
    public class AsyncCommand : BaseCommand
    {
        private readonly Func<CancellationToken, Task> _ExecuteAsync;
        private readonly Predicate<object> _CanExecute;

        #region IsNowExecuting

        private bool _IsNowExecuting;

        /// <summary> Выполняется ли команда в текущий момент </summary>
        public bool IsNowExecuting
        {
            get => _IsNowExecuting;
            private set
            {
                _IsNowExecuting = value;
                CommandManager.InvalidateRequerySuggested();
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
                _CancelSource = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Отмена выполнения команды </summary>
        public void CancelExecute() => CancelSource?.Cancel();

        #endregion


        public AsyncCommand(Func<Task> ExecuteAsync, Predicate<object> CanExecute = null) : this(_ => ExecuteAsync(), CanExecute)
        {
        }

        public AsyncCommand(Func<CancellationToken, Task> ExecuteAsync, Predicate<object> CanExecute)
        {
            _ExecuteAsync = ExecuteAsync;
            _CanExecute = CanExecute;
        }

        protected override bool CanExecuteCommand(object P) => !IsNowExecuting && (_CanExecute?.Invoke(P) ?? true);

        protected override async void ExecuteCommand(object P)
        {
            try
            {
                CancelSource ??= new();

                IsNowExecuting = true;

                await _ExecuteAsync(CancelSource.Token).ConfigureAwait(true);
            }
            catch (OperationCanceledException) { }
            finally
            {
                IsNowExecuting = false;
                CancelSource = null;
            }
        }
    }
}