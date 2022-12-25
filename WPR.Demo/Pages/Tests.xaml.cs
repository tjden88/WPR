using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPR.MVVM.Commands;
using WPR.MVVM.Commands.Base;

namespace WPR.Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для Tests.xaml
    /// </summary>
    public partial class Tests : Page
    {
        public Tests()
        {
            InitializeComponent();
        }

        #region Command TestCommand - Тестовая команда

        /// <summary>Тестовая команда</summary>
        private Command _TestCommand;

        /// <summary>Тестовая команда</summary>
        public Command TestCommand => _TestCommand
            ??= new Command(OnTestCommandExecuted, CanTestCommandExecute, "Тестовая команда", new KeyGesture(Key.A, ModifierKeys.Control), Application.Current.MainWindow);

        /// <summary>Проверка возможности выполнения - Тестовая команда</summary>
        private bool CanTestCommandExecute() => true;

        /// <summary>Логика выполнения - Тестовая команда</summary>
        private void OnTestCommandExecuted()
        {
            WPRMessageBox.Information(this, "asdad");
        }

        #endregion
    }
}
