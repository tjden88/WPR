using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPR.MVVM.Commands
{
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

        #region Ctor

        protected AsyncCommand()
        {
        }

        public AsyncCommand(Func<CancellationToken, Task> ExecuteAsync, Func<bool> CanExecute = null, string CommandText = null) :
            this(ExecuteAsync, CanExecute, CommandText, null, null)
        {
        }

        public AsyncCommand(Func<object, CancellationToken, Task> ExecuteAsync, Predicate<object> CanExecute = null, string CommandText = null) :
            this(ExecuteAsync, CanExecute, CommandText, null, null)
        {
        }

        public AsyncCommand(Func<CancellationToken, Task> ExecuteAsync, Func<bool> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget) :
            this((_, t) => ExecuteAsync(t), CanExecute is null ? null : _ => CanExecute(), CommandText, ExecuteGesture, GestureTarget)
        {
        }

        public AsyncCommand(Func<object, CancellationToken, Task> ExecuteAsync, Predicate<object> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget)
        {
            _ExecuteAsync = ExecuteAsync;
            _CanExecute = CanExecute;
            Text = CommandText;

            this.ExecuteGesture = ExecuteGesture;
            GestureTarget?.InputBindings.Add(new InputBinding(this, ExecuteGesture));
        }

        #endregion

        protected override bool CanExecuteCommand(object P) => !IsNowExecuting && (_CanExecute?.Invoke(P) ?? true);

        protected override async void ExecuteCommand(object P)
        {
            try
            {
                CancelSource ??= new();

                IsNowExecuting = true;

                await _ExecuteAsync(P, CancelSource.Token).ConfigureAwait(true);
            }
            catch (OperationCanceledException) { }
            finally
            {
                IsNowExecuting = false;
                CancelSource = null;
            }
        }
    }



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


        protected override bool CanExecuteCommand(object P)
        {
            if (!CanExecuteWithNullParameter && P is not T) return false;
            return _CanExecute?.Invoke((T)P) ?? true;
        }

        protected override async void ExecuteCommand(object P)
        {
            try
            {
                CancelSource ??= new();

                IsNowExecuting = true;

                await _ExecuteAsync((T) P, CancelSource.Token).ConfigureAwait(true);
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