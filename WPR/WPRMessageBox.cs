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

        #region CancelInformation

        /// <summary>Показать диалог с кнопками ОК, ОТМЕНА</summary>
        public static void CancelInformation(DependencyObject sender, string Caption, Action<bool> Callback = null)
        {
            CancelInformation(sender, Caption, string.Empty, Callback);
        }
        /// <summary>Показать диалог с кнопками ОК, ОТМЕНА</summary>
        public static void CancelInformation(DependencyObject sender, string Caption, string Title, Action<bool> Callback = null)
        {
            Show(sender, Caption, Title, B => Callback?.Invoke(B ?? false), true, false);
        }
        #endregion

        #region CancelInformationAsync

        /// <summary>Показать диалог с кнопками ОК, ОТМЕНА</summary>
        public static async Task<bool> CancelInformationAsync(DependencyObject sender, string Caption)
        {
            return await CancelInformationAsync(sender, Caption, string.Empty);
        }
        /// <summary>Показать диалог с кнопками ОК, ОТМЕНА</summary>
        public static async Task<bool> CancelInformationAsync(DependencyObject sender, string Caption, string Title)
        {
            TaskCompletionSource<bool> complete = new();
            CancelInformation(sender, Caption, Title, (b) => complete.TrySetResult(b));
            return await complete.Task.ConfigureAwait(false);
        }
        #endregion

        #region ShowQuestion

        

        #endregion
    }
}
