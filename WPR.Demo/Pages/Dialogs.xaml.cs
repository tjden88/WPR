using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPR.Dialogs;
using WPR.MVVM.Commands.Base;
using WPR.Theme;

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
            //WPRDialogHelper.Information(this, "Текст диалога пользователя", "Заголовок", () => Debug.WriteLine("Диалог закрыт"));
            //WPRDialogHelper.InformationCancel(this, "Текст диалога пользователя", "Заголовок", (b) => Debug.WriteLine($"Диалог закрыт: {b}"));
            //WPRDialogHelper.Question(this, "Текст диалога пользователя", "Заголовок", (b) => Debug.WriteLine($"Диалог закрыт: {b}"));
        }

        #endregion

        #region Command ShowWindowDialogCommandAsync - Показать диалог асинхронно

        private ICommand _ShowWindowDialogCommandAsync;

        /// <summary>Показать диалог асинхронно</summary>
        public ICommand ShowWindowDialogCommandAsync => _ShowWindowDialogCommandAsync
            ??= new Command(OnShowWindowDialogCommandAsyncExecuted);

        private async void OnShowWindowDialogCommandAsyncExecuted()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var dialog = new MessageDialog()
            {
                Title = "Заголовок",
                Content = "Текст диалога пользователя",
                DialogType = DialogType.QuestionCancel
            };
            await Task.Delay(100).ConfigureAwait(false);
            var result = await UserDialogHelper.Show(this, dialog, cts.Token).ConfigureAwait(false);
            //var result = await UserDialogHelper.ShowModal(this, dialog, cts.Token).ConfigureAwait(false);
            Debug.WriteLine(result);
        }

        #endregion

        #region Command ShowModalDialogCommand - Показать модальный диалог

        private ICommand _ShowModalDialogCommand;

        /// <summary>Показать модальный диалог</summary>
        public ICommand ShowModalDialogCommand => _ShowModalDialogCommand
            ??= new Command(OnShowModalDialogCommandExecuted);

        private void OnShowModalDialogCommandExecuted()
        {
        }

        #endregion



        private async void Dlg_OnClick(object Sender, RoutedEventArgs E)
        {
            var dlg = new WPRUserDialog();

            var msg = "Сообщение";
            var title = "Заголовок";

            await dlg.InformationAsync(msg, title);


            var val = new List<(Predicate<string> rule, string errorMessage)>()
            {
                new(s => !string.IsNullOrEmpty(s), "Обязательно"),
                new(s => s?.Length > 2, "Больше 2"),
            };

            var coolFilter = new InputDialogFilterOptions()
                    .AddRequired()
                    .AddDefaultValue("123")
                    .AddMessage("Это сообщение")
                    .AddMinLen(3)
                    .AddMaxLen(10)
                    .AddMustNotContains(["123", "456"])
                    .AddRule(s => s?.StartsWith("789") ?? true, "Должно начинаться с 789")
                ;
            await dlg.InputValidatedTextAsync(options =>
            {  
                options
                    .AddTitle("Ввод с валидацией")
                    .AddDefaultValue("123")
                    .AddMessage("Это сообщение")
                    .AddRequired().AddMinLen(3)
                    .AddMaxLen(10)
                    .SetMultiline()
                    .AddMustNotContains(new[] { "123", "456" })
                    .AddRule(s => s?.StartsWith("789") ?? true, "Должно начинаться с 789")
                    ;

            });
            await dlg.ShowNotificationAsync("Задержка 5 сек", StyleBrushes.AccentColorBrush, 5000);

        }


    }
}
