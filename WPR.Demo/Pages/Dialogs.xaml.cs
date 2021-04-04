using System.Diagnostics;
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
            MsgBox.DialogResult += DialogResult;
        }

        private void DialogResult(bool? Obj)
        {
            Debug.WriteLine(Obj);
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
            ??= new Command(OnShowWindowDialogCommandExecuted);

        private void OnShowWindowDialogCommandExecuted()
        {
            //WPRMessageBox.Information(this, "Текст диалога пользователя", "Заголовок", () => Debug.WriteLine("Диалог закрыт"));
            WPRMessageBox.CancelInformation(this, "Текст диалога пользователя", "Заголовок", (b) => Debug.WriteLine($"Диалог закрыт: {b}"));
        }

        #endregion

        #region Command ShowWindowDialogCommandAsync - Показать диалог асинхронно

        private ICommand _ShowWindowDialogCommandAsync;

        /// <summary>Показать диалог асинхронно</summary>
        public ICommand ShowWindowDialogCommandAsync => _ShowWindowDialogCommandAsync
            ??= new Command(OnShowWindowDialogCommandAsyncExecuted);

        private async void OnShowWindowDialogCommandAsyncExecuted()
        {
            //await WPRMessageBox.InformationAsync(this, "Текст диалога пользователя");
          var res= await WPRMessageBox.CancelInformationAsync(this, "Текст диалога пользователя");
            Debug.WriteLine($"Асинхронный диалог закрыт: {res}");
        }

        #endregion
    }
}
