using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPR.Controls;

namespace WPR
{
    /// <summary>Диалоговые окна</summary>
    public static class WPRMessageBox
    {
        #region Private
        // Найти панель для отображения диалога
        private static WPRDialogPanel FindDialogPanel(DependencyObject uIElement)
        {
            if (uIElement is WPRDialogPanel panel)
            {
                return panel;
            }

            if (uIElement is Window window)
            {
                return window.Template.FindName("WindowDialogPanel", window) as WPRDialogPanel;
            }

            return uIElement.FindVisualParent<WPRDialogPanel>();
        }

        private static void Show(DependencyObject sender, string Caption, string Title, Action<bool?> Callback, bool CancelButton, bool YesNoButtons)
        {
            // Ищем панель
            WPRDialogPanel panel = FindDialogPanel(sender);

            WPRMsgBox messageBox = new()
            {
                Title = Title,
                Caption = Caption,
                CancelButtonVisible = CancelButton,
                YesNoButtonsVisible = YesNoButtons
            };
            // При клике по кнопке мессаджа закрыть окно и вернуть прозрачность как была
            messageBox.DialogResult += (b) =>
            {
                panel.Hide();
                Callback?.Invoke(b);
            };
            panel.Show(messageBox, true);
        }

        #endregion


        #region Information
        /// <summary>Показать информационное окно</summary>
        public static void Information(DependencyObject sender, string Caption, Action Callback = null)
        {
            Information(sender, Caption, string.Empty, Callback);
        }

        /// <summary>Показать информационное окно</summary>
        public static void Information(DependencyObject sender, string Caption, string Title, Action Callback = null)
        {
            Show(sender, Caption,Title, B => Callback?.Invoke(), false, false);
        }
        #endregion

        #region InformationAsync
        /// <summary>Показать информационное окно</summary>
        public static async Task InformationAsync(DependencyObject sender, string Caption)
        {
            await InformationAsync(sender, Caption, string.Empty).ConfigureAwait(false);
        }

        /// <summary>Показать информационное окно</summary>
        public static async Task InformationAsync(DependencyObject sender, string Caption, string Title)
        {
            TaskCompletionSource complete = new();

            Information(sender, Caption, Title, () => complete.TrySetResult());
            await complete.Task.ConfigureAwait(false);
        }
        #endregion

        #region InformationCancel

        /// <summary>Показать диалог с кнопками ОК, ОТМЕНА</summary>
        public static void InformationCancel(DependencyObject sender, string Caption, Action<bool> Callback = null)
        {
            InformationCancel(sender, Caption, string.Empty, Callback);
        }
        /// <summary>Показать диалог с кнопками ОК, ОТМЕНА</summary>
        public static void InformationCancel(DependencyObject sender, string Caption, string Title, Action<bool> Callback = null)
        {
            Show(sender, Caption, Title, B => Callback?.Invoke(B ?? false), true, false);
        }
        #endregion

        #region InformationCancelAsync

        /// <summary>Показать диалог с кнопками ОК, ОТМЕНА</summary>
        public static async Task<bool> InformationCancelAsync(DependencyObject sender, string Caption)
        {
            return await InformationCancelAsync(sender, Caption, string.Empty);
        }
        /// <summary>Показать диалог с кнопками ОК, ОТМЕНА</summary>
        public static async Task<bool> InformationCancelAsync(DependencyObject sender, string Caption, string Title)
        {
            TaskCompletionSource<bool> complete = new();
            InformationCancel(sender, Caption, Title, (b) => complete.TrySetResult(b));
            return await complete.Task.ConfigureAwait(false);
        }
        #endregion

        #region Question
        /// <summary>Показать диалог с кнопками ДА, НЕТ</summary>
        public static void Question(DependencyObject sender, string Caption, Action<bool> Callback = null)
        {
            Question(sender, Caption, string.Empty, Callback);
        }

        /// <summary>Показать диалог с кнопками ДА, НЕТ</summary>
        public static void Question(DependencyObject sender, string Caption, string Title, Action<bool> Callback = null)
        {
            Show(sender, Caption, Title, B => Callback?.Invoke(B ?? false), false, true);
        }
        #endregion

        #region QuestionAsync
        /// <summary>Показать диалог с кнопками ДА, НЕТ</summary>
        public static async Task<bool> QuestionAsync(DependencyObject sender, string Caption)
        {
            return await QuestionAsync(sender, Caption, string.Empty);
        }
        /// <summary>Показать диалог с кнопками ДА, НЕТ</summary>
        public static async Task<bool> QuestionAsync(DependencyObject sender, string Caption, string Title)
        {
            TaskCompletionSource<bool> complete = new();
            Question(sender, Caption, Title, (b) => complete.TrySetResult(b));
            return await complete.Task.ConfigureAwait(false);
        }

        #endregion

        #region QuestionCancel
        /// <summary>Показать диалог с кнопками ДА, НЕТ, ОТМЕНА</summary>
        public static void QuestionCancel(DependencyObject sender, string Caption, Action<bool?> Callback = null)
        {
            QuestionCancel(sender, Caption, string.Empty, Callback);
        }

        /// <summary>Показать диалог с кнопками ДА, НЕТ, ОТМЕНА</summary>
        public static void QuestionCancel(DependencyObject sender, string Caption, string Title, Action<bool?> Callback = null)
        {
            Show(sender, Caption, Title, Callback, true, true);
        }
        #endregion

        #region QuestionCancelAsync

        /// <summary>Показать диалог с кнопками ДА, НЕТ, ОТМЕНА</summary>
        public static async Task<bool?> QuestionCancelAsync(DependencyObject sender, string Caption)
        {
            return await QuestionCancelAsync(sender, Caption, string.Empty);
        }
        /// <summary>Показать диалог с кнопками ДА, НЕТ, ОТМЕНА</summary>
        public static async Task<bool?> QuestionCancelAsync(DependencyObject sender, string Caption, string Title)
        {
            TaskCompletionSource<bool?> complete = new();
            QuestionCancel(sender, Caption, Title, (b) => complete.TrySetResult(b));
            return await complete.Task.ConfigureAwait(false);
        }
        #endregion

        #region Error

        /// <summary>Показать информационное окно с сообщением об ошибке</summary>
        public static void Error(DependencyObject sender, Exception e, Action Callback = null)
        {
            Show(sender, e.Message, "Ошибка!", B => Callback?.Invoke(), false, false);
        }
        #endregion

        #region ErrorAsync

        /// <summary>Показать информационное окно с сообщением об ошибке</summary>
        public static async Task ErrorAsync(DependencyObject sender, Exception e)
        {
            TaskCompletionSource complete = new();

            Error(sender, e, () => complete.TrySetResult());
            await complete.Task.ConfigureAwait(false);
        }
        #endregion
    }
}
