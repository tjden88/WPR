﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPR.Dialogs;
using WPR.Dialogs.Base;
using WPR.MVVM.Commands.Base;

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
            WPRDialogHelper.InputText(null, // Модальное
                "Ввод текста:",
                (B, S) => {if(B) Debug.WriteLine(S);},
                "Стартовое значение",
                S => S.Length>0,
                "Поле не может быть пустым");
        }
        private void Button4_OnClick(object Sender, RoutedEventArgs E)
        {
            WPRDialogHelper.InputText(this,
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
    }
}
