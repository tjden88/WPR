using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WPR.Controls;
using WPR.Dialogs;
using WPR.MVVM.Validation;

namespace WPR
{
    /// <summary>Диалоговые окна</summary>
    public static class WPRMessageBox
    {
        #region Private
        // Найти панель для отображения диалога
        private static WPRDialogPanel FindDialogPanel(DependencyObject uIElement)
        {
            if (uIElement == null) return null;

            if (uIElement is Window window)
            {
                return window?.Template?.FindName("WindowDialogPanel", window) as WPRDialogPanel;
            }

            return uIElement.FindVisualParent<WPRDialogPanel>();
        }

        private static void Show(DependencyObject sender, string Caption, string Title, Action<bool?> Callback, bool CancelButton, bool YesNoButtons)
        {
            // Ищем панель
            WPRDialogPanel panel = FindDialogPanel(sender);
            if (panel == null)
            {
                throw new ArgumentNullException(nameof(panel), "Окно с панелью диалога не найдено!");
            }

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

        #region ShowDialog

        /// <summary>
        /// Показать как модальное окно
        /// </summary>
        /// <param name="sender">Объект, окно которого будет розительским по отношению к диалогу.
        /// Если null, диалог будет поверх всех окон</param>
        /// <param name="Caption">Сообщение</param>
        /// <param name="Title">Заголовок</param>
        /// <param name="CancelButton">Показать кнопку отмены</param>
        /// <param name="YesNoButtons">Показать кнопки Да/Нет вместо ОК</param>
        /// <returns></returns>
        public static bool? ShowModal(DependencyObject sender, string Caption, string Title = "", bool CancelButton = false, bool YesNoButtons = false)
        {
            bool? result = null;

            WPRMsgBox messageBox = new()
            {
                Title = Title,
                Caption = Caption,
                CancelButtonVisible = CancelButton,
                YesNoButtonsVisible = YesNoButtons
            };

            var owner = sender is Window w ? w : sender?.FindVisualParent<Window>();

            var panel = FindDialogPanel(owner);

            Window dlg = new()
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = owner,
                Topmost = owner == null,
                Content = messageBox,
                Style = (Style)Application.Current.Resources["WPRModalWindow"]
            };

            messageBox.DialogResult += B =>
            {
                result = B;
                dlg.Close();
            };

            panel?.Show(null,true);

            dlg.ShowDialog();

            panel?.Hide();

            return result;
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

        #region Bubble

        /// <summary>Показать всплывающее сообщение</summary>
        public static void Bubble(DependencyObject sender, string Text, int Duration = 3000)
        {
            var p = FindDialogPanel(sender);
            p?.ShowBubble(Text, Duration);
        }

        /// <summary>Показать всплывающее сообщение с кнопкой</summary>
        public static void Bubble(DependencyObject sender, string Text, string ButtonText, Action<bool> Callback, int Duration = 4000)
        {
            var p = FindDialogPanel(sender);
            p?.ShowBubble(Text, Duration, ButtonText, Callback);
        }
        #endregion

        #region InputBoxes

        public static void InputText(DependencyObject sender, string Title, Action<bool, string> Callback, string DefaultValue = "")
        {
            InputText(sender, Title, Callback, DefaultValue, S => true);
        }

        public static void InputText(DependencyObject sender, string Title, Action<bool, string> Callback, string DefaultValue , Predicate<string> ValidationRule , string ValidationErrorMessage = "Неверное значение")
        {
            var rule = new PredicateValidationRule<string>(ValidationRule, ValidationErrorMessage);

            InputText(sender, Title, Callback, DefaultValue, new[] {rule});
        }

        public static void InputText(DependencyObject sender, string Title, Action<bool, string> Callback, string DefaultValue, IEnumerable<PredicateValidationRule<string>> ValidationRules)
        {
            // Ищем панель
            WPRDialogPanel panel = FindDialogPanel(sender);

            WPRInputBox inputBox = new(ValidationRules)
            {
                Title = Title,
                TextValue = DefaultValue
            };
            // При клике по кнопке мессаджа закрыть окно и вернуть прозрачность как была
            inputBox.DialogResult += (b) =>
            {
                panel.Hide();
                Callback?.Invoke(b == true, inputBox.TextValue);
            };
            panel.Show(inputBox, true);
        }


        /// <summary> Поле ввода текста </summary>
        public static Task<string> InputTextAsync(DependencyObject sender, string Title, string DefaultValue = "") => 
            InputTextAsync(sender, Title, DefaultValue, S => true);


        /// <summary> Поле ввода текста с валидацией </summary>
        public static Task<string> InputTextAsync(DependencyObject sender, string Title, string DefaultValue, Predicate<string> ValidationRule, string ValidationErrorMessage = "Неверное значение") => 
            InputTextAsync(sender, Title, DefaultValue, new []{new PredicateValidationRule<string>(ValidationRule, ValidationErrorMessage) });


        /// <summary> Поле ввода текста с коллекцией валидаций </summary>
        public static async Task<string> InputTextAsync(DependencyObject sender, string Title, string DefaultValue, IEnumerable<PredicateValidationRule<string>> ValidationRules)
        {
            TaskCompletionSource<string> complete = new();
            InputText(sender, Title, (B, S) => { complete.TrySetResult(B ? S : null); }, DefaultValue, ValidationRules);
            return await complete.Task.ConfigureAwait(false);
        }

        #endregion

        #region CustomDialog

        /// <summary>Показать диалог с кастомным содержимым</summary>
        public static void ShowCustomDialog(DependencyObject sender, IWPRDialog Content, Action<bool> Callback = null)
        {
            // Ищем панель
            WPRDialogPanel panel = FindDialogPanel(sender);

            Content.DialogResult += (b) =>
            {
                panel.Hide();
                Callback?.Invoke(b);
            };
            panel.Show(Content, Content.StaysOpen);
        }

        /// <summary>Показать диалог с кастомным содержимым</summary>
        public static async Task<bool> ShowCustomDialogAsync(DependencyObject sender, IWPRDialog Content)
        {
            TaskCompletionSource<bool> complete = new();
            ShowCustomDialog(sender, Content, (b) => complete.TrySetResult(b));
            return await complete.Task.ConfigureAwait(false);
        }

        #endregion
    }
}
