using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPR.MVVM.Commands
{
    public class AsyncCommand : BaseCommand
    {
        private readonly Action<object, CancellationToken> _Execute;
        private readonly Predicate<object> _CanExecute;

        private bool _IsNowExecuting;
        private CancellationTokenSource _CancelSource;

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

        /// <summary> Токен отмены операции </summary>
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
        public void CancelExecute() => CancelSource?.Cancel(true);

        public AsyncCommand(Action<object, CancellationToken> Execute, Predicate<object> CanExecute = null, string CommandText = null)
        : this(Execute, CanExecute, CommandText, null, null)
        {
        }

        public AsyncCommand(Action<CancellationToken> Execute, Func<bool> CanExecute = null, string CommandText = null)
            : this( Execute, CanExecute , CommandText, null, null)
        {
        }

        public AsyncCommand(Action<CancellationToken> Execute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget) :
            this(Execute, null,  CommandText, ExecuteGesture, GestureTarget)
        {
        }

        public AsyncCommand(Action<CancellationToken> Execute, Func<bool> CanExecute, string CommandText, KeyGesture ExecuteGesture,
            UIElement GestureTarget) : this((_, Source) => Execute.Invoke(Source), CanExecute is null ? null : P => CanExecute(), CommandText, ExecuteGesture, GestureTarget)
        {
        }

        public AsyncCommand(Action<object, CancellationToken> Execute, Predicate<object> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
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
                CancelSource ??= new CancellationTokenSource();
                IsNowExecuting = true;
                await Task.Run(() => _Execute(P, CancelSource.Token)).ConfigureAwait(true);
                IsNowExecuting = false;
                CancelSource = null;
            }
            catch (OperationCanceledException)
            {
                IsNowExecuting = false;
                CancelSource = null;
            }
        }
    }
}