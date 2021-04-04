using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Converters;

namespace WPR.Controls
{
    public class WPRMsgBox: Control
    {
        /// <summary>Действие при вводе пользователя</summary>
        public Action<bool?> DialogResult;

        static WPRMsgBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRMsgBox), new FrameworkPropertyMetadata(typeof(WPRMsgBox)));
        }

        public WPRMsgBox()
        {
            SetDialogResultCommand = new ResultCommand(this);
        }

        #region Title : string - Заголовок сообщения

        /// <summary>Заголовок сообщения</summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(WPRMsgBox),
                new PropertyMetadata(default(string)));

        /// <summary>Заголовок сообщения</summary>
        [Description("Заголовок сообщения")]
        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region Caption : string - Текст сообщения

        /// <summary>Текст сообщения</summary>
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(
                nameof(Caption),
                typeof(string),
                typeof(WPRMsgBox),
                new PropertyMetadata(default(string)));

        /// <summary>Текст сообщения</summary>
        //[Category("")]
        [Description("Текст сообщения")]
        public string Caption
        {
            get => (string) GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        #endregion

        #region CancelButtonVisible : bool - Видимость кнопки отмены

        /// <summary>Видимость кнопки отмены</summary>
        public static readonly DependencyProperty CancelButtonVisibleProperty =
            DependencyProperty.Register(
                nameof(CancelButtonVisible),
                typeof(bool),
                typeof(WPRMsgBox),
                new PropertyMetadata(default(bool)));

        /// <summary>Видимость кнопки отмены</summary>
        //[Category("")]
        [Description("Видимость кнопки отмены")]
        public bool CancelButtonVisible
        {
            get => (bool) GetValue(CancelButtonVisibleProperty);
            set => SetValue(CancelButtonVisibleProperty, value);
        }

        #endregion

        #region YesNoButtonsVisible : bool - Видимость кнопок Да,Нет. Если false - видна кнопка OK

        /// <summary>Видимость кнопок Да,Нет. Если false - видна кнопка OK</summary>
        public static readonly DependencyProperty YesNoButtonsVisibleProperty =
            DependencyProperty.Register(
                nameof(YesNoButtonsVisible),
                typeof(bool),
                typeof(WPRMsgBox),
                new PropertyMetadata(default(bool)));

        /// <summary>Видимость кнопок Да,Нет. Если false - видна кнопка OK</summary>
        //[Category("")]
        [Description("Видимость кнопок Да,Нет. Если false - видна кнопка OK")]
        public bool YesNoButtonsVisible
        {
            get => (bool)GetValue(YesNoButtonsVisibleProperty);
            set => SetValue(YesNoButtonsVisibleProperty, value);
        }

        #endregion


        #region SetDialogResultCommand : ICommand - Команда нажатия контрольных кнопок

        /// <summary>Команда нажатия контрольных кнопок</summary>
        public static readonly DependencyProperty SetDialogResultCommandProperty =
            DependencyProperty.Register(
                nameof(SetDialogResultCommand),
                typeof(ICommand),
                typeof(WPRMsgBox),
                new PropertyMetadata(null));

        /// <summary>Команда нажатия контрольных кнопок</summary>
        //[Category("")]
        [Description("Команда нажатия контрольных кнопок")]
        public ICommand SetDialogResultCommand
        {
            get => (ICommand) GetValue(SetDialogResultCommandProperty);
            set => SetValue(SetDialogResultCommandProperty, value);
        }

        #endregion

        class ResultCommand: ICommand
        {
            private readonly WPRMsgBox _MsgBox;

            public ResultCommand(WPRMsgBox msgBox)
            {
                _MsgBox = msgBox;
            }
            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                bool? result = (bool?)parameter;
                _MsgBox.DialogResult?.Invoke(result);
            }

            public event EventHandler CanExecuteChanged;
        }
    }
}
