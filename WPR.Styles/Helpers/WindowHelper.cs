using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WPR.Styles.Helpers
{
    public static class WindowHelper
    {
        /// <summary>
        /// Отступ строки заголовка от левого края
        /// </summary>
        public static readonly DependencyProperty TitleLeftMarginProperty =
            DependencyProperty.RegisterAttached("TitleLeftMargin", typeof(double), typeof(WindowHelper), new PropertyMetadata(2.0));

        public static void SetTitleLeftMargin(DependencyObject obj, double value)
        {
            obj?.SetValue(TitleLeftMarginProperty, value);
        }

        public static double GetTitleLeftMargin(DependencyObject obj)
        {
            return (double?) obj?.GetValue(TitleLeftMarginProperty) ?? 0.0;
        }



        public static Brush GetWindowHeaderBackGround(DependencyObject obj)
        {
            return (Brush)obj.GetValue(WindowHeaderBackGroundProperty);
        }

        public static void SetWindowHeaderBackGround(DependencyObject obj, Brush value)
        {
            obj.SetValue(WindowHeaderBackGroundProperty, value);
        }

        /// <summary>
        /// Фон строки заголовка окна
        /// </summary>
        public static readonly DependencyProperty WindowHeaderBackGroundProperty =
            DependencyProperty.RegisterAttached("WindowHeaderBackGround", typeof(Brush), typeof(WindowHelper), new PropertyMetadata(null));



        public static Brush GetWindowHeaderForeground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(WindowHeaderForegroundProperty);
        }

        public static void SetWindowHeaderForeground(DependencyObject obj, Brush value)
        {
            obj.SetValue(WindowHeaderForegroundProperty, value);
        }

        /// <summary>
        /// Цвет текста строки заголовка окна
        /// </summary>
        public static readonly DependencyProperty WindowHeaderForegroundProperty =
            DependencyProperty.RegisterAttached("WindowHeaderForeground", typeof(Brush), typeof(WindowHelper), new PropertyMetadata(null));
    }
}
