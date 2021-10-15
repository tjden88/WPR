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

        /// <summary> Выполняется ли команда в текущий момент </summary>
        public bool IsNowExecuting { get; private set; }

        public CancellationTokenSource CancelSource { get; set; }

        public AsyncCommand(Action<object, CancellationToken> Execute, Predicate<object> CanExecute = null, string CommandText = null)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _CanExecute = CanExecute;
            Text = CommandText;
        }

        public AsyncCommand(Action<CancellationToken> Execute, Func<bool> CanExecute = null, string CommandText = null)
            : this((O, Source) => Execute.Invoke(Source), CanExecute is null ? null : P => CanExecute(), CommandText)
        {
        }

        public AsyncCommand(Action<CancellationToken> Execute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget) :
            this(Execute, CommandText, ExecuteGesture, GestureTarget)
        {
        }

        public AsyncCommand(Action<CancellationToken> Execute, Func<bool> CanExecute, string CommandText, KeyGesture ExecuteGesture,
            UIElement GestureTarget) : this(Execute, CanExecute, CommandText, ExecuteGesture, GestureTarget)
        {
        }

        public AsyncCommand(Action<object, CancellationToken> Execute, Predicate<object> CanExecute, string CommandText,
            KeyGesture ExecuteGesture, UIElement GestureTarget) : this(Execute, CanExecute, CommandText)
        {
        }

        protected override bool CanExecuteCommand(object P) => !IsNowExecuting && base.CanExecuteCommand(P);

        protected override async void ExecuteCommand(object P)
        {
            try
            {
                CancelSource ??= new CancellationTokenSource();
                IsNowExecuting = true;
                await Task.Run(() => _Execute(P, CancelSource.Token));
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


    public class Command2 : BaseCommand
    {
        private readonly Action<object, CancellationToken> _Execute;
        private readonly Predicate<object> _CanExecute;

        public CancellationTokenSource Cancel;

        public Command2(Action<object, CancellationToken> Execute, Predicate<object> CanExecute = null, string CommandText = null)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _CanExecute = CanExecute;
            Text = CommandText;
        }

        public Command2(Action<CancellationToken> Execute, Func<bool> CanExecute = null, string CommandText = null)
            : this( (O, Source) => Execute.Invoke(Source), CanExecute is null ? null : P => CanExecute(), CommandText)
        {
        }

        /// <summary>Возможность выполнения команды</summary>
        protected override bool CanExecuteCommand(object P) => _CanExecute?.Invoke(P) ?? true;

        /// <summary>Выполнить команду</summary>
        protected override async void ExecuteCommand(object P)
        {
            Cancel = new();

            await Task.Run(() => _Execute(P, Cancel.Token));
        }
    }

}