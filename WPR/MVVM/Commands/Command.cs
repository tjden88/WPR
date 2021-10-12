using System;
using System.Windows;
using System.Windows.Input;

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

        public Command(Action Execute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget) 
            : this(P => Execute(), null, CommandText, ExecuteGesture, GestureTarget)
        {
        }
        public Command(Action Execute, Func<bool> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget)
            : this(P => Execute(), P => CanExecute(), CommandText, ExecuteGesture, GestureTarget)
        {
        }

        public Command(Action<object> Execute, Predicate<object> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget)
            : this(Execute, CanExecute)
        {
            if (GestureTarget == null) throw new ArgumentNullException(nameof(GestureTarget));
            Text = CommandText;
            this.ExecuteGesture = ExecuteGesture;
            GestureTarget.InputBindings.Add(new InputBinding(this, ExecuteGesture));
        }

        /// <summary>Возможность выполнения команды</summary>
        public override bool CanExecute(object P) => _CanExecute?.Invoke(P) ?? true;

        /// <summary>Выполнить команду</summary>
        public override void Execute(object P) => _Execute(P);

    }
}
