using System.Windows;

namespace WPR.Helpers;

public static class WindowHelper
{
    
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