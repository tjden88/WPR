using System;

namespace WPR.MVVM.Commands
{
    public class Command : BaseCommand
    {
        private readonly Action<object> _Execute;
        private readonly Predicate<object> _CanExecute;


        public Command(Action Execute, Func<bool> CanExecute = null) : this(P => Execute(),
            CanExecute is null ? null : P => CanExecute())
        {
        }

        public Command(Action<object> Execute, Predicate<object> CanExecute = null)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _CanExecute = CanExecute;
        }

        /// <summary>Возможность выполнения команды</summary>
        protected override bool CanExecute(object P) => _CanExecute?.Invoke(P) ?? true;

        /// <summary>Выполнить команду</summary>
        protected override void Execute(object P) => _Execute(P);

    }
}
