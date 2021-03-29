using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WPR.Styles.Helpers
{
    public static class ButtonHelper
    {
        #region Радиус бордера
        /// <summary>
        /// Радиус бордера элементов
        /// </summary>
        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyProperty.RegisterAttached("BorderRadius", typeof(CornerRadius), typeof(ButtonHelper), new PropertyMetadata(default(CornerRadius)));

        public static void SetBorderRadius(UIElement element, CornerRadius value) => element?.SetValue(BorderRadiusProperty, value);

        public static CornerRadius GetBorderRadius(UIElement element) => (CornerRadius)element?.GetValue(BorderRadiusProperty);

        #endregion

        #region Будет ли обрезаться контент внутри ClippingBorder

        /// <summary>
        /// Будет ли обрезаться контент внутри ClippingBorder
        /// </summary>
        public static bool GetBorderClipContent(DependencyObject obj) => (bool)obj?.GetValue(BorderClipContentProperty);

        public static void SetBorderClipContent(DependencyObject obj, bool value) => obj?.SetValue(BorderClipContentProperty, value);

        public static readonly DependencyProperty BorderClipContentProperty =
            DependencyProperty.RegisterAttached("BorderClipContent", typeof(bool), typeof(ButtonHelper), new PropertyMetadata(true));
        #endregion
    }
}
