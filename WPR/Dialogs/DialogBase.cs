using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPR.Dialogs
{
    public abstract class DialogBase : Control
    {
        /// <summary>Происходит при завершении ввода пользователя</summary>
        public Action<bool?> DialogResult;

        protected DialogBase()
        {
            SetDialogResultCommand = new ResultCommand(this);
            CancelCommand = new CancCommand(this);
        }

        #region Title : string - Заголовок

        /// <summary>Заголовок</summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(DialogBase),
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
                typeof(DialogBase),
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

        #region CancelCommand : ICommand - Команда отмены

        /// <summary>Команда отмены</summary>
        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register(
                nameof(CancelCommand),
                typeof(ICommand),
                typeof(DialogBase),
                new PropertyMetadata(null));

        /// <summary>Команда отмены</summary>
        //[Category("")]
        [Description("Команда отмены")]
        public ICommand CancelCommand
        {
            get => (ICommand)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }
        #endregion

        /// <summary>Вызывается при срабатывании команды SetDialogResultCommand</summary>
        protected virtual void OnSetCommandExecute(bool parameter) => DialogResult?.Invoke(parameter);

        /// <summary>Может ли выполниться команда SetDialogResultCommand</summary>
        protected virtual bool CanSetCommandExecuted() => true;

        private class ResultCommand : ICommand
        {
            private readonly DialogBase _Dialog;

            public ResultCommand(DialogBase dialog)
            {
               _Dialog = dialog;
            }
            public bool CanExecute(object parameter) => _Dialog.CanSetCommandExecuted();

            public void Execute(object parameter)
            {
                bool result = (bool)parameter;
                _Dialog.OnSetCommandExecute(result);
            }
            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }

        private class CancCommand : ICommand
        {
            private readonly DialogBase _Dialog;

            public CancCommand(DialogBase dialog)
            {
                _Dialog = dialog;
            }
            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                _Dialog.DialogResult?.Invoke(null);
            }
            public event EventHandler CanExecuteChanged;
        }
    }
}
