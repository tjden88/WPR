using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPR.MVVM.Commands
{
    /// <summary>
    /// Базовая асинхронная реализация команды.
    /// Формирует задачу и запускает её
    /// </summary>
    public class AsyncCommand : BaseCommand
    {
        private readonly Action<object, CancellationToken> _ExecuteAsync;
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

        private CancellationTokenSource _TempCancellationTokenSource; // Для одноразовых токенов
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


        public AsyncCommand(Action<object, CancellationToken> ExecuteAsync, Predicate<object> CanExecute = null, string CommandText = null)
        : this(ExecuteAsync, CanExecute, CommandText, null, null)
        {
        }

        public AsyncCommand(Action<CancellationToken> ExecuteAsync, Func<bool> CanExecute = null, string CommandText = null)
            : this(ExecuteAsync, CanExecute, CommandText, null, null)
        {
        }

        public AsyncCommand(Action<CancellationToken> ExecuteAsync, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget) :
            this(ExecuteAsync, null, CommandText, ExecuteGesture, GestureTarget)
        {
        }

        public AsyncCommand(Action<CancellationToken> ExecuteAsync, Func<bool> CanExecute, string CommandText, KeyGesture ExecuteGesture,
            UIElement GestureTarget) : this((_, Source) => ExecuteAsync.Invoke(Source), CanExecute is null ? null : P => CanExecute(), CommandText, ExecuteGesture, GestureTarget)
        {
        }

        public AsyncCommand(Action<object, CancellationToken> ExecuteAsync, Predicate<object> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget)
        {
            _ExecuteAsync = ExecuteAsync ?? throw new ArgumentNullException(nameof(ExecuteAsync));
            _CanExecute = CanExecute;
            Text = CommandText;

            this.ExecuteGesture = ExecuteGesture;
            GestureTarget?.InputBindings.Add(new InputBinding(this, ExecuteGesture));
        }

        protected override bool CanExecuteCommand(object P) => !IsNowExecuting && (_CanExecute?.Invoke(P) ?? true);

        protected override async void ExecuteCommand(object P)
        {
            try
            {
                if (CancelSource == null)
                {
                    _TempCancellationTokenSource = new();
                    CancelSource = _TempCancellationTokenSource;
                }
                IsNowExecuting = true;
                await Task.Run(() => _ExecuteAsync(P, CancelSource.Token)).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                IsNowExecuting = false;
                CancelSource = null;
                _TempCancellationTokenSource?.Dispose();
                _TempCancellationTokenSource = null;
            }
        }
    }
}