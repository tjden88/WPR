using System;
using System.Threading;
using System.Threading.Tasks;

namespace WPR.MVVM.Commands
{
    public class CommandAsync : BaseCommand
    {
        private readonly Func<object, Task> _ExecuteAsync;
        private readonly Predicate<object> _CanExecute;

        private bool _IsExecutingNow;

        public CommandAsync(Func<Task> ExecuteAsync, Func<bool> CanExecute = null) 
            : this(ExecuteAsync is null ? throw new ArgumentNullException(nameof(ExecuteAsync)) : new Func<object, Task>(_ => ExecuteAsync()),
            CanExecute is null ? null : P => CanExecute())
        {
        }

        public CommandAsync(Func<object, Task> ExecuteAsync, Predicate<object> CanExecute = null)
        {
            _ExecuteAsync = ExecuteAsync ?? throw new ArgumentNullException(nameof(ExecuteAsync));
            _CanExecute = CanExecute;
        }

        /// <summary>Возможность выполнения команды</summary>
        protected override bool CanExecuteCommand(object P) => !_IsExecutingNow && (_CanExecute?.Invoke(P) ?? true);

        /// <summary>Выполнить команду</summary>
        protected override async void ExecuteCommand(object Parameter)
        {
            try
            {
                _IsExecutingNow = true;
                await _ExecuteAsync(Parameter).ConfigureAwait(true);
                _IsExecutingNow = false;
            }
            catch (OperationCanceledException)
            {
                _IsExecutingNow = false;
            }
        }
    }

    public class Command2 : BaseCommand
    {
        private readonly Action<object> _Execute;
        private readonly Predicate<object> _CanExecute;

        public CancellationTokenSource Cancel;

        public Command2(Action<object> Execute, Predicate<object> CanExecute = null, string CommandText = null)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _CanExecute = CanExecute;
            Text = CommandText;
        }

        public Command2(Action Execute, Func<bool> CanExecute = null, string CommandText = null)
            : this(P => Execute(), CanExecute is null ? null : P => CanExecute(), CommandText)
        {
        }

        /// <summary>Возможность выполнения команды</summary>
        protected override bool CanExecuteCommand(object P) => _CanExecute?.Invoke(P) ?? true;

        /// <summary>Выполнить команду</summary>
        protected override async void ExecuteCommand(object P)
        {
            Cancel = new();

            await Task.Run(() => _Execute(P), Cancel.Token);
        }
    }

}