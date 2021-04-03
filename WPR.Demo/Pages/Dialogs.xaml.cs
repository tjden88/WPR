using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPR.Controls;
using WPR.Demo.Commands.Base;

namespace WPR.Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для Dialogs.xaml
    /// </summary>
    public partial class Dialogs : Page
    {
        public Dialogs()
        {
            InitializeComponent();
        }

        #region Command ShowUserDialogCommand - Показать диалог пользователя

        private ICommand _ShowUserDialogCommand;

        /// <summary>Показать диалог пользователя</summary>
        public ICommand ShowUserDialogCommand => _ShowUserDialogCommand
            ??= new Command(OnShowUserDialogCommandExecuted, CanShowUserDialogCommandExecute);

        private bool CanShowUserDialogCommandExecute() => true;

        private void OnShowUserDialogCommandExecuted()
        {
            DialogPanel.Show(false);
        }

        #endregion

        #region Command ShowWindowDialogCommand - Показать диалог окна

        private ICommand _ShowWindowDialogCommand;

        /// <summary>Показать диалог окна</summary>
        public ICommand ShowWindowDialogCommand => _ShowWindowDialogCommand
            ??= new Command(OnShowWindowDialogCommandExecuted, CanShowWindowDialogCommandExecute);

        private bool CanShowWindowDialogCommandExecute() => true;

        private void OnShowWindowDialogCommandExecuted()
        {
            WPRDialogPanel.ShowOnWindow(Application.Current.MainWindow, "sdklfjsklwsefmes", false);
        }

        #endregion
    }
}
