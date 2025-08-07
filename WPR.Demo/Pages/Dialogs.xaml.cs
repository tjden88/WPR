using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPR.Demo.Models;
using WPR.Dialogs;
using WPR.Theme;

namespace WPR.Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для Dialogs.xaml
    /// </summary>
    public partial class Dialogs : Page
    {
        private readonly IUserDialog _UserDialog;
        public IUserDialog PanelDlg { get; }


        public Dialogs()
        {
            PanelDlg = DialogHelper.CreateUserDialog();
            InitializeComponent();
            _UserDialog = DialogHelper.Default;
        }

        private async Task ShowNotification(object message) =>
            await _UserDialog.ShowNotificationAsync(message?.ToString() ?? "null");


        private async void Dlg_OnClick(object Sender, RoutedEventArgs E)
        {
            var dlg = _UserDialog;

            var result = await dlg.InputValidatedTextAsync(options =>
            {  
                options
                    .AddTitle("Ввод с валидацией")
                    .AddDefaultValue("123")
                    .AddMessage("Это сообщение")
                    .AddRequired()
                    .AddMinLen(3)
                    .AddMaxLen(10)
                    .SetMultiline()
                    .AddMustNotContains(["123", "456"])
                    .AddRule(s => s?.StartsWith("789") ?? true, "Должно начинаться с 789")
                    ;

            });

            await dlg.ShowNotificationAsync(result, StyleBrushes.AccentColorBrush, 5000);

        }


        private void Button1_OnClick(object sender, RoutedEventArgs e)
        { 
            _UserDialog.InformationAsync("Текст сообщения", "Заголовок");
        }

        private async void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            await ShowNotification(await _UserDialog.QuestionAsync("Вопрос", "Заголовок"));
        }

        private async void Button3_OnClick(object sender, RoutedEventArgs e)
        {
            await ShowNotification(await _UserDialog.QuestionAsync("Вопрос с отменой", DialogType.QuestionCancel, "Заголовок"));
        }

        private async void Button4_OnClick(object sender, RoutedEventArgs e)
        {
            await ShowNotification(await _UserDialog.CustomQuestionAsync("Кастомные кнопки", "Заголовок", "Принять!", "Завернуть!", "Отмена нах!"));
        }
        private void Button5_OnClick(object sender, RoutedEventArgs e)
        {
            _UserDialog.ErrorMessageAsync("Текст сообщения ошибки");
        }

        private void Button6_OnClick(object sender, RoutedEventArgs e)
        {
            DialogHelper.ModalDialog.InformationAsync("Модальное окно", "Модальный заголовок");
        }

        private async void Button7_OnClick(object sender, RoutedEventArgs e)
        {
            await ShowNotification(await _UserDialog.InputTextAsync("Контент", "123", "Заголовок"));
        }
        private async void Button8_OnClick(object sender, RoutedEventArgs e)
        {
            await ShowNotification(await _UserDialog.InputTextAsync("Контент мульти-инпута", "123", "Заголовок", true));
        }

        private async void Button9_OnClick(object sender, RoutedEventArgs e)
        {
            await PanelDlg.InformationAsync("Внутри панели");
            var pressed = await PanelDlg.ShowQuestionNotificationAsync("Нотификация с кнопкой", "Push!");
            await PanelDlg.ShowNotificationAsync(pressed.ToString(), StyleBrushes.SuccessColorBrush);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogPanel.DialogSource = PanelDlg;
        }

        private readonly Person _Person = new("Иван", 30);

        private async void Button10_OnClick(object sender, RoutedEventArgs e)
        {
            using var context = new WprDialogViewModel(_Person);
            var dlg = new WprDialog
            {
                DataContext = context
            };
            context.DialogContent = dlg;

            var result = await _UserDialog.CustomDialogAsync(context);
        }
    }
}
