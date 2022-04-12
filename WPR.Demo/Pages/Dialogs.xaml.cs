using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPR.MVVM.Commands;

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

        #region Command ShowWindowDialogCommand - Показать диалог окна

        private ICommand _ShowWindowDialogCommand;

        /// <summary>Показать диалог окна</summary>
        public ICommand ShowWindowDialogCommand => _ShowWindowDialogCommand
            ??= new Command(OnShowWindowDialogCommandExecuted);

        private void OnShowWindowDialogCommandExecuted()
        {
            //WPRMessageBox.Information(this, "Текст диалога пользователя", "Заголовок", () => Debug.WriteLine("Диалог закрыт"));
            //WPRMessageBox.InformationCancel(this, "Текст диалога пользователя", "Заголовок", (b) => Debug.WriteLine($"Диалог закрыт: {b}"));
            //WPRMessageBox.Question(this, "Текст диалога пользователя", "Заголовок", (b) => Debug.WriteLine($"Диалог закрыт: {b}"));
            WPRMessageBox.QuestionCancel(this, "Текст диалога пользователя", "Заголовок", (b) => Debug.WriteLine($"Диалог закрыт: {b}"));
        }

        #endregion

        #region Command ShowWindowDialogCommandAsync - Показать диалог асинхронно

        private ICommand _ShowWindowDialogCommandAsync;

        /// <summary>Показать диалог асинхронно</summary>
        public ICommand ShowWindowDialogCommandAsync => _ShowWindowDialogCommandAsync
            ??= new Command(OnShowWindowDialogCommandAsyncExecuted);

        private async void OnShowWindowDialogCommandAsyncExecuted()
        {
            await WPRMessageBox.InformationAsync(this, "Текст диалога пользователя");
            //var res = await WPRMessageBox.InformationCancelAsync(this, "Текст диалога пользователя");
            //var res = await WPRMessageBox.QuestionAsync(this, "Текст диалога пользователя");
            var res = await WPRMessageBox.QuestionCancelAsync(this, "Текст диалога пользователя");
            Debug.WriteLine($"Асинхронный диалог закрыт: {res}");
        }

        #endregion

        #region Command ShowModalDialogCommand - Показать модальный диалог

        private ICommand _ShowModalDialogCommand;

        /// <summary>Показать модальный диалог</summary>
        public ICommand ShowModalDialogCommand => _ShowModalDialogCommand
            ??= new Command(OnShowModalDialogCommandExecuted);

        private void OnShowModalDialogCommandExecuted()
        {
            Debug.WriteLine(WPRMessageBox.ShowModal(this, "Модальный диалог", "Заголовок", true));
        }

        #endregion

        private void ButtonBase_OnClick(object Sender, RoutedEventArgs E)
        {
            WPRMessageBox.Bubble(this, "Всплывающее сообщение");
        }

        private void Button2_OnClick(object Sender, RoutedEventArgs E)
        {
            WPRMessageBox.Bubble(this, "Всплывающее сообщение с кнопкой", "YEP!", _ => Debug.WriteLine("Clicked!"));
        }

        private void Button3_OnClick(object Sender, RoutedEventArgs E)
        {
            WPRMessageBox.InputText(this,
                "Ввод текста:",
                (B, S) => {if(B) Debug.WriteLine(S);},
                "Стартовое значение",
                S => S.Length>0,
                "Поле не может быть пустым");
        }
        private void Button4_OnClick(object Sender, RoutedEventArgs E)
        {
            WPRMessageBox.InputText(this,
                "Ввод текста:",
                (B, S) => { Debug.WriteLine(B + S); },
                "12",
                S => S.Length > 3,
                "Нужно больше 3 символов");
        }

        class TestCustomDialog : IWPRDialog
        {
            private readonly Dialogs _Parent;
            private int _Count;
            public Action<bool> DialogResult { get; set; }

            public object DialogContent { get; set; }
            public bool StaysOpen => false;

            public TestCustomDialog(Dialogs parent, int count)
            {
                _Parent = parent;
                _Count = count;
                DialogContent = new Button()
                {
                    Content =$"Запустить ещё один диалог. Текущий: {count}",
                    Command = new Command(() => WPRMessageBox.ShowCustomDialog(parent, new TestCustomDialog(parent, count + 1)))
                };
            }
        }

        private async void CustomDialog_Click(object Sender, RoutedEventArgs E)
        {
            Debug.WriteLine(await WPRMessageBox.ShowCustomDialogAsync(this, new TestCustomDialog(this, 0)));
        }


    }
}
