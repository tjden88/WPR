using System.Windows;

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
            DependencyProperty.RegisterAttached("Description", typeof(object), typeof(TextHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static void SetDescription(UIElement element, object value) => element?.SetValue(DescriptionProperty, value);

        public static object GetDescription(UIElement element) => element?.GetValue(DescriptionProperty);

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

    }
}
