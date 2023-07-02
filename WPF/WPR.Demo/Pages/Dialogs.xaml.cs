using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPR.Dialogs;
using WPR.Domain.Interfaces;
using WPR.Domain.Models.Dialogs;
using WPR.Domain.Models.Dialogs.Extensions;
using WPR.Domain.Models.Themes;
using WPR.MVVM.Commands;
using WPR.UiServices.UI;

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
            //WPRDialogHelper.Information(this, "Текст диалога пользователя", "Заголовок", () => Debug.WriteLine("Диалог закрыт"));
            //WPRDialogHelper.InformationCancel(this, "Текст диалога пользователя", "Заголовок", (b) => Debug.WriteLine($"Диалог закрыт: {b}"));
            //WPRDialogHelper.Question(this, "Текст диалога пользователя", "Заголовок", (b) => Debug.WriteLine($"Диалог закрыт: {b}"));
            WPRDialogHelper.QuestionCancel(this, "Текст диалога пользователя", "Заголовок", (b) => Debug.WriteLine($"Диалог закрыт: {b}"));
        }

        #endregion

        #region Command ShowWindowDialogCommandAsync - Показать диалог асинхронно

        private ICommand _ShowWindowDialogCommandAsync;

        /// <summary>Показать диалог асинхронно</summary>
        public ICommand ShowWindowDialogCommandAsync => _ShowWindowDialogCommandAsync
            ??= new Command(OnShowWindowDialogCommandAsyncExecuted);

        private async void OnShowWindowDialogCommandAsyncExecuted()
        {
            await WPRDialogHelper.InformationAsync(this, "Текст диалога пользователя");
            //var res = await WPRDialogHelper.InformationCancelAsync(this, "Текст диалога пользователя");
            //var res = await WPRDialogHelper.QuestionAsync(this, "Текст диалога пользователя");
            var res = await WPRDialogHelper.QuestionCancelAsync(this, "Текст диалога пользователя");
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
            Debug.WriteLine(WPRDialogHelper.ShowModal(this, "Модальный диалог", "Заголовок", true));
        }

        #endregion

        private void ButtonBase_OnClick(object Sender, RoutedEventArgs E)
        {
            WPRDialogHelper.Bubble(this, "Всплывающее сообщение");
        }

        private void Button2_OnClick(object Sender, RoutedEventArgs E)
        {
            WPRDialogHelper.Bubble(this, "Всплывающее сообщение с кнопкой", "YEP!", _ => Debug.WriteLine("Clicked!"));
        }

        private void Button3_OnClick(object Sender, RoutedEventArgs E)
        {
            WPRDialogHelper.InputText(null,// Модальное
                "Ввод текста:",
                (B, S) => {if(B) Debug.WriteLine(S);},
                "Описание",
                "Стартовое значение",
                S => S.Length>0,
                "Поле не может быть пустым");
        }
        private void Button4_OnClick(object Sender, RoutedEventArgs E)
        {
            WPRDialogHelper.InputText(this,
                "Ввод текста:",
                (B, S) => { Debug.WriteLine(B + S); },
                "Описание",
                "12",
                S => S?.Length > 3,
                "Нужно больше 3 символов");
        }

        class TestCustomDialog : IWPRDialog
        {
            private readonly Dialogs _Parent;
            private int _Count;

            public Action<bool> SetDialogResult { get; set; }

            public object DialogContent { get; set; }
            public bool StaysOpen => false;

            public TestCustomDialog(Dialogs parent, int count)
            {
                _Parent = parent;
                _Count = count;
                DialogContent = new Button()
                {
                    Content =$"Запустить ещё один диалог. Текущий: {count}",
                    Command = new Command(() => WPRDialogHelper.ShowCustomDialog(parent, new TestCustomDialog(parent, count + 1)))
                };
            }
        }

        private async void CustomDialog_Click(object Sender, RoutedEventArgs E)
        {
            Debug.WriteLine(await WPRDialogHelper.ShowCustomDialogAsync(this, new TestCustomDialog(this, 0)));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await WPRDialogHelper.ShowCustomDialogAsync(this, new WprDialog());
            WPRDialogHelper.ShowCustomDialog(null, new WprDialog(), b => Debug.WriteLine(b));
        }

        private async void Dlg_OnClick(object Sender, RoutedEventArgs E)
        {
            var dlg = new UserDialog(new AppNavigationService(null!));

            var msg = "Сообщение";
            var title = "Заголовок";

            await dlg.InformationAsync(msg, title);

            Debug.WriteLine(await dlg.QuestionAsync(msg, title));
            Debug.WriteLine(await dlg.QuestionAsync(msg, IUserDialog.DialogTypes.YesNo, title));
            Debug.WriteLine(await dlg.QuestionAsync(msg, IUserDialog.DialogTypes.OkCancel, title));
            Debug.WriteLine(await dlg.QuestionAsync(msg, IUserDialog.DialogTypes.YesNoCancel, title));

            Debug.WriteLine(await dlg.CustomQuestionAsync(msg, title, "true"));
            Debug.WriteLine(await dlg.CustomQuestionAsync(msg, title, "true", "false"));
            Debug.WriteLine(await dlg.CustomQuestionAsync(msg, title, "true", null, "null"));
            Debug.WriteLine(await dlg.CustomQuestionAsync(msg, title, "true", "false", "null"));

            await dlg.ErrorMessageAsync(msg, title);

            Debug.WriteLine(await dlg.CustomDialogAsync(new WprDialog()));

            Debug.WriteLine(await dlg.InputTextAsync(title));
            Debug.WriteLine(await dlg.InputTextAsync(title, "123", msg));

            var val = new List<(Predicate<string> rule, string errorMessage)>()
            {
                new(s => !string.IsNullOrEmpty(s), "Обязательно"),
                new(s => s?.Length > 2, "Больше 2"),
            };

            var coolFilter = new InputDialogFilter("Тест офигенного фильтра")
                    .AddRequired()
                    .AddDefaultValue("123")
                    .AddMessage("Это сообщение")
                    .AddMinLen(3)
                    .AddMaxLen(10)
                    .AddMustNotContains(new[] { "123", "456" })
                    .AddRule(s => s?.StartsWith("789") ?? true, "Должно начинаться с 789")
                ;
            await dlg.InputValidatedTextAsync(coolFilter);
            await dlg.ShowNotificationAsync("Задержка 5 сек", 5000);

            Debug.WriteLine(await dlg.ShowQuestionNotificationAsync(msg, title));
        }

        private void ButtonBase3_OnClick(object Sender, RoutedEventArgs E)
        {
            WPRDialogHelper.Bubble(this, "Danger Dialog", "OK", null, 1000, StyleBrushes.DangerColorBrush);
            WPRDialogHelper.Bubble(this, "Accent Dialog", "OK", null, 1000, StyleBrushes.AccentColorBrush);
            WPRDialogHelper.Bubble(this, "Success Dialog", "OK", null, 1000, StyleBrushes.SuccessColorBrush);
        }
    }
}
