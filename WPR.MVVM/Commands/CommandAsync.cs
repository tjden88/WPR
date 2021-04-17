using System;
using System.Threading.Tasks;

namespace WPR.MVVM.Commands
{
    public class CommandAsync : BaseCommand
    {
        private readonly Action<object> _Execute;
        private readonly Predicate<object> _CanExecute;


        public CommandAsync(Action Execute, Func<bool> CanExecute = null) : this(P => Execute(),
            CanExecute is null ? null : P => CanExecute())
        {
        }

        public CommandAsync(Action<object> Execute, Predicate<object> CanExecute = null)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _CanExecute = CanExecute;
        }

        /// <summary>Возможность выполнения команды</summary>
        protected override bool CanExecute(object P) => _CanExecute?.Invoke(P) ?? true;

        /// <summary>Выполнить команду</summary>
        protected override async void Execute(object Parameter)
        {
            if (!CanExecute(Parameter)) return;
            try
            {
                Executable = false;
                await Task.Run(() => _Execute(Parameter));
                Executable = true;
            }
            catch
            {
                Executable = true;
                throw;
            }
        }
    }
}