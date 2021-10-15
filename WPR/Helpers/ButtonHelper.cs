using System.Windows;
using System.Windows.Media.Effects;

namespace WPR.Helpers
{
    public static class ButtonHelper
    {
        #region Радиус бордера
        /// <summary>
        /// Радиус бордера элементов
        /// </summary>
        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyProperty.RegisterAttached("BorderRadius", typeof(CornerRadius), typeof(ButtonHelper), new PropertyMetadata(default(CornerRadius)));

        public static void SetBorderRadius(UIElement element, CornerRadius value) => element.SetValue(BorderRadiusProperty, value);

        public static CornerRadius GetBorderRadius(UIElement element) => (CornerRadius)element.GetValue(BorderRadiusProperty);

        #endregion

        #region Запретить Ripple выходить за границы кнопки

        /// <summary>
        /// Запретить Ripple выходить за границы кнопки
        /// </summary>
        public static bool GetNoRippleCircleOutside(DependencyObject obj) => (bool)obj.GetValue(NoRippleCircleOutsideProperty);

        public static void SetNoRippleCircleOutside(DependencyObject obj, bool value) => obj.SetValue(NoRippleCircleOutsideProperty, value);

        public static readonly DependencyProperty NoRippleCircleOutsideProperty =
            DependencyProperty.RegisterAttached("NoRippleCircleOutside", typeof(bool), typeof(ButtonHelper), new PropertyMetadata(true));
        #endregion

        #region Attached property ButtonShadow : DropShadowEffect - Тень кнопки

        /// <summary>Тень кнопки</summary>
        public static readonly DependencyProperty ButtonShadowProperty =
            DependencyProperty.RegisterAttached(
                "ButtonShadow",
                typeof(DropShadowEffect),
                typeof(ButtonHelper),
                new PropertyMetadata(default(DropShadowEffect)));

        /// <summary>Тень кнопки</summary>
        public static void SetButtonShadow(DependencyObject d, DropShadowEffect value) => d.SetValue(ButtonShadowProperty, value);

        /// <summary>Тень кнопки</summary>
        public static DropShadowEffect GetButtonShadow(DependencyObject d) => (DropShadowEffect) d.GetValue(ButtonShadowProperty);

        #endregion  
    }
}
