using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPR.Controls.Base
{
    public abstract class Dialog : Control
    {
        /// <summary>Происходит при завершении ввода пользователя</summary>
        public Action<bool?> DialogResult;

        protected Dialog()
        {
            SetDialogResultCommand = new ResultCommand(this);
        }

        #region Title : string - Заголовок

        /// <summary>Заголовок</summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(Dialog),
                new PropertyMetadata(default(string)));

        /// <summary>Заголовок</summary>
        [Description("Заголовок")]
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region SetDialogResultCommand : ICommand - Команда нажатия контрольных кнопок

        /// <summary>Команда нажатия контрольных кнопок</summary>
        public static readonly DependencyProperty SetDialogResultCommandProperty =
            DependencyProperty.Register(
                nameof(SetDialogResultCommand),
                typeof(ICommand),
                typeof(Dialog),
                new PropertyMetadata(null));

        /// <summary>Команда нажатия контрольных кнопок</summary>
        //[Category("")]
        [Description("Команда нажатия контрольных кнопок")]
        public ICommand SetDialogResultCommand
        {
            get => (ICommand)GetValue(SetDialogResultCommandProperty);
            set => SetValue(SetDialogResultCommandProperty, value);
        }
        #endregion

        /// <summary>Вызывается при срабатывании команды SetDialogResultCommand</summary>
        protected virtual void OnSetCommandExecute(bool? parameter)
        {
            DialogResult?.Invoke(parameter);
        }

        class ResultCommand : ICommand
        {
            private readonly Dialog _Dialog;

            public ResultCommand(Dialog dialog)
            {
               _Dialog = dialog;
            }
            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                bool? result = (bool?)parameter;
                _Dialog.OnSetCommandExecute(result);
            }

            public event EventHandler CanExecuteChanged;
        }
    }
}
