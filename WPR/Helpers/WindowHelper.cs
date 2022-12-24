using System.Windows;
using System.Windows.Media;

namespace WPR.Helpers;

public static class WindowHelper
{

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


    #region Контент заголовка
    public static object GetWindowHeaderContent(DependencyObject obj)
    {
        return obj.GetValue(WindowHeaderContentProperty);
    }

    public static void SetWindowHeaderContent(DependencyObject obj, object value)
    {
        obj.SetValue(WindowHeaderContentProperty, value);
    }

    /// <summary> Контент заголовка </summary>
    public static readonly DependencyProperty WindowHeaderContentProperty =
        DependencyProperty.RegisterAttached("WindowHeaderContent", typeof(object), typeof(WindowHelper), new PropertyMetadata(null));
    #endregion

}