using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPR.Helpers
{
    public static class TextHelper
    {
        #region Подсказка для тестбоксов
        /// <summary>
        /// Подсказка для тестбоксов
        /// </summary>
        public static readonly DependencyProperty HintProperty =
            DependencyProperty.RegisterAttached("Hint", typeof(object), typeof(TextHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static void SetHint(UIElement element, object value) => element?.SetValue(HintProperty, value);

        public static object GetHint(UIElement element) => element?.GetValue(HintProperty);

        #endregion

        #region Описание текстбоксов
        /// <summary>
        /// Описание под элементом (текстбокс)
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.RegisterAttached("Description", typeof(object), typeof(TextHelper), 
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static void SetDescription(UIElement element, object value) => element.SetValue(DescriptionProperty, value);

        public static object GetDescription(UIElement element) => (object)element.GetValue(DescriptionProperty);

        #endregion

        #region Attached property ShowClearButton : bool - Показать ли кнопку очистки текстбокса

        /// <summary>Показать ли кнопку очистки текстбокса</summary>
        public static readonly DependencyProperty ShowClearButtonProperty =
            DependencyProperty.RegisterAttached(
                "ShowClearButton",
                typeof(bool),
                typeof(TextHelper),
                new PropertyMetadata(default(bool)));

        /// <summary>Показать ли кнопку очистки текстбокса</summary>
        public static void SetShowClearButton(DependencyObject d, bool value) => d.SetValue(ShowClearButtonProperty, value);

        /// <summary>Показать ли кнопку очистки текстбокса</summary>
        public static bool GetShowClearButton(DependencyObject d) => (bool) d.GetValue(ShowClearButtonProperty);

        #endregion

        #region Attached property AddValidationRule : ValidationRule - Добавить правило валидации к текстбоксу

        /// <summary>Добавить правило валидации к текстбоксу</summary>
        public static readonly DependencyProperty AddValidationRuleProperty =
            DependencyProperty.RegisterAttached(
                "AddValidationRule",
                typeof(ValidationRule),
                typeof(TextHelper),
                new PropertyMetadata(default(ValidationRule), AddValidationRuleOnPropertyChanged));

        private static void AddValidationRuleOnPropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
        {
            if (D is not TextBox textBox)
                throw new NotSupportedException("Свойство предназначено для TextBox");

            if (E.OldValue is ValidationRule oldValue)
            {
                var binding = BindingOperations.GetBinding(textBox, TextBox.TextProperty);
                binding?.ValidationRules.Remove(oldValue);
            }

            if (E.NewValue is ValidationRule newValue)
            {
                var binding = BindingOperations.GetBinding(textBox, TextBox.TextProperty);
                if (binding != null)
                {
                    binding.ValidationRules.Add(newValue);
                    textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                    //textBox.GotFocus += (_, _) =>  textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                }
            }

        }

        /// <summary>Добавить правило валидации к текстбоксу</summary>
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static void SetAddValidationRule(DependencyObject d, ValidationRule value) => d.SetValue(AddValidationRuleProperty, value);

        /// <summary>Добавить правило валидации к текстбоксу</summary>
        public static ValidationRule GetAddValidationRule(DependencyObject d) => (ValidationRule) d.GetValue(AddValidationRuleProperty);

        #endregion
    }
}
