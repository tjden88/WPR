using System.Windows;
using System.Windows.Input;
using WPR.MVVM.Commands.Base;

namespace WPR.MVVM.Commands
{
    /// <summary>
    /// Реализация базовой команды
    /// </summary>
    public class Command : BaseCommand
    {
        private readonly Action<object> _Execute;
        private readonly Predicate<object> _CanExecute;

        public Command(Action<object> Execute, Predicate<object> CanExecute = null, string CommandText = null)
            : this(Execute, CanExecute, CommandText, null, null)
        {
        }

        public Command(Action Execute, Func<bool> CanExecute = null, string CommandText = null)
            : this(Execute, CanExecute , CommandText, null, null)
        {
        }

        public Command(Action Execute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget) 
            : this(Execute, null, CommandText, ExecuteGesture, GestureTarget)
        {
        }

        public Command(Action Execute, Func<bool> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget)
            : this(P => Execute(), CanExecute is null ? null : _ => CanExecute(), CommandText, ExecuteGesture, GestureTarget)
        {
        }

        public Command(Action<object> Execute, Predicate<object> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _CanExecute = CanExecute;
            Text = CommandText;

            this.ExecuteGesture = ExecuteGesture;
            GestureTarget?.InputBindings.Add(new InputBinding(this, ExecuteGesture));
        }

        /// <summary>Возможность выполнения команды</summary>
        protected override bool CanExecuteCommand(object P) => _CanExecute?.Invoke(P) ?? true;

        /// <summary>Выполнить команду</summary>
        protected override void ExecuteCommand(object P) => _Execute(P);
    }





    /// <summary>
    /// Типизированная базовая реализация команды
    /// </summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    public class Command<T> : BaseCommand
    {
        private readonly Action<T> _Execute;
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

        public Command(Action<T> Execute, Predicate<T> CanExecute = null, string CommandText = null)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _CanExecute = CanExecute;
            Text = CommandText;
        }

        public Command(Action<T> Execute, Predicate<T> CanExecute, string CommandText, KeyGesture ExecuteGesture, UIElement GestureTarget) 
            : this(Execute, CanExecute, CommandText)
        {
            this.ExecuteGesture = ExecuteGesture;
            GestureTarget?.InputBindings.Add(new InputBinding(this, ExecuteGesture));
        }

        /// <summary>Возможность выполнения команды</summary>
        protected override bool CanExecuteCommand(object P)
        {

            if (!CanExecuteWithNullParameter && P is not T) return false;

            return _CanExecute?.Invoke((T)P) ?? true;
        }

        /// <summary>Выполнить команду</summary>
        protected override void ExecuteCommand(object P) => _Execute((T)P);
    }
}
