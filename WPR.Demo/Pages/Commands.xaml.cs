using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using WPR.MVVM.Commands;

namespace WPR.Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для Commands.xaml
    /// </summary>
    public partial class Commands : Page
    {
        public Commands()
        {
            InitializeComponent();
        }

        private int _Count;

        private void ShowBubble(string text) => WPRMessageBox.Bubble(this, text);

        #region Command SimpleCommand - Простая команда

        /// <summary>Простая команда</summary>
        private Command _SimpleCommand;

        /// <summary>Простая команда</summary>
        public Command SimpleCommand => _SimpleCommand
            ??= new Command(OnSimpleCommandExecuted, CanSimpleCommandExecute);

        /// <summary>Проверка возможности выполнения - Простая команда</summary>
        private bool CanSimpleCommandExecute() => true;

        /// <summary>Логика выполнения - Простая команда</summary>
        private void OnSimpleCommandExecuted()
        {
            ShowBubble("Простая команда");
            SimpleCommand.Executable = false;
        }

        #endregion

        #region Command CommandWithTextCommand - Команда с именем

        /// <summary>Команда с именем</summary>
        private Command _CommandWithTextCommand;

        /// <summary>Команда с именем</summary>
        public Command CommandWithTextCommand => _CommandWithTextCommand
            ??= new Command(OnCommandWithTextCommandExecuted, CanCommandWithTextCommandExecute, "Команда с текстом");

        /// <summary>Проверка возможности выполнения - Команда с именем</summary>
        private bool CanCommandWithTextCommandExecute() => true;

        /// <summary>Логика выполнения - Команда с именем</summary>
        private void OnCommandWithTextCommandExecuted()
        {
            ShowBubble(CommandWithTextCommand.Text);
            _Count++;
            CommandWithTextCommand.Text += _Count;
        }

        #endregion

        #region Command CommandWithKeyGestureCommand - Команда с горячей клавишей

        /// <summary>Команда с горячей клавишей</summary>
        private Command _CommandWithKeyGestureCommand;

        /// <summary>Команда с горячей клавишей</summary>
        public Command CommandWithKeyGestureCommand => _CommandWithKeyGestureCommand
            ??= new Command(OnCommandWithKeyGestureCommandExecuted, CanCommandWithKeyGestureCommandExecute, "Команда с горячей клавишей",
                new KeyGesture(Key.A, ModifierKeys.Control), this);

        /// <summary>Проверка возможности выполнения - Команда с горячей клавишей</summary>
        private bool CanCommandWithKeyGestureCommandExecute() => true;

        /// <summary>Логика выполнения - Команда с горячей клавишей</summary>
        private void OnCommandWithKeyGestureCommandExecuted()
        {
            ShowBubble(CommandWithKeyGestureCommand.ToString());
        }

        #endregion

        #region Command GenericCommand - Типизированная команда

        /// <summary>Типизированная команда</summary>
        private Command<string> _GenericCommand;

        /// <summary>Типизированная команда</summary>
        public Command<string> GenericCommand => _GenericCommand
            ??= new Command<string>(OnGenericCommandExecuted, CanGenericCommandExecute, "Типизированная команда");

        /// <summary>Проверка возможности выполнения - Типизированная команда</summary>
        private bool CanGenericCommandExecute(string s) => s != string.Empty;

        /// <summary>Логика выполнения - Типизированная команда</summary>
        private void OnGenericCommandExecuted(string s)
        {
            ShowBubble(s);
            GenericCommand.CanExecuteWithNullParameter = true;
        }

        #endregion

        #region Command SimpleAsyncCommand - Асинхронная команда

        /// <summary>Асинхронная команда</summary>
        private Command2 _SimpleAsyncCommand;

        /// <summary>Асинхронная команда</summary>
        public Command2 SimpleAsyncCommand => _SimpleAsyncCommand
            ??= new Command2(OnSimpleAsyncCommandExecuted, CanSimpleAsyncCommandExecute, "Асинхронная команда");

        /// <summary>Проверка возможности выполнения - Асинхронная команда</summary>
        private bool CanSimpleAsyncCommandExecute() => true;

        /// <summary>Логика выполнения - Асинхронная команда</summary>
        private void OnSimpleAsyncCommandExecuted()
        {
            VeryLongTask();
        }

        #endregion

        #region Command CancelAsyncCommand - Отменить асинхронную команду

        /// <summary>Отменить асинхронную команду</summary>
        private Command _CancelAsyncCommand;

        /// <summary>Отменить асинхронную команду</summary>
        public Command CancelAsyncCommand => _CancelAsyncCommand
            ??= new Command(OnCancelAsyncCommandExecuted, CanCancelAsyncCommandExecute, "Отменить асинхронную команду");

        /// <summary>Проверка возможности выполнения - Отменить асинхронную команду</summary>
        private bool CanCancelAsyncCommandExecute() => true;

        /// <summary>Логика выполнения - Отменить асинхронную команду</summary>
        private void OnCancelAsyncCommandExecuted()
        {
            SimpleAsyncCommand.Cancel.Cancel(true);
        }

        #endregion



        private void VeryLongTask()
        {
            this.DoDispatherAction(() => ShowBubble("Very Long Task Was Started"));
            Thread.Sleep(5000);
            this.DoDispatherAction(() => ShowBubble("Very Long Task Completed!"));
        }
    }
}
