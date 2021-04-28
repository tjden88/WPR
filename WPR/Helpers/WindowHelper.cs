using System.Windows;
using System.Windows.Media;

namespace WPR.Helpers
{
    public static class WindowHelper
    {
        #region  Отступ строки заголовка от левого края 
        /// <summary> Отступ строки заголовка от левого края </summary>
        public static readonly DependencyProperty TitleLeftMarginProperty =
            DependencyProperty.RegisterAttached("TitleLeftMargin", typeof(double), typeof(WindowHelper), new PropertyMetadata(2.0));

        public static void SetTitleLeftMargin(DependencyObject obj, double value)
        {
            obj?.SetValue(TitleLeftMarginProperty, value);
        }

        public static double GetTitleLeftMargin(DependencyObject obj)
        {
            return (double?)obj?.GetValue(TitleLeftMarginProperty) ?? 0.0;
        }

        #endregion

        #region Цвет текста строки заголовка окна
        public static SolidColorBrush GetWindowHeaderForeground(DependencyObject obj)
        {
            return (SolidColorBrush)obj.GetValue(WindowHeaderForegroundProperty);
        }

        public static void SetWindowHeaderForeground(DependencyObject obj, SolidColorBrush value)
        {
            obj.SetValue(WindowHeaderForegroundProperty, value);
        }

        /// <summary> Цвет текста строки заголовка окна </summary>
        public static readonly DependencyProperty WindowHeaderForegroundProperty =
            DependencyProperty.RegisterAttached("WindowHeaderForeground", typeof(SolidColorBrush), typeof(WindowHelper), new PropertyMetadata(null)); 
        #endregion

    }
}
