using System.Windows;
using System.Windows.Media;

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


    #region Attached property HeaderButtonsForeground : Brush - Цвет служебных кнопок в заголовке

    /// <summary>Цвет служебных кнопок в заголовке</summary>
    public static readonly DependencyProperty HeaderButtonsForegroundProperty =
        DependencyProperty.RegisterAttached(
            "HeaderButtonsForeground",
            typeof(Brush),
            typeof(WindowHelper),
            new PropertyMetadata(default(Brush)));

    /// <summary>Цвет служебных кнопок в заголовке</summary>
    public static void SetHeaderButtonsForeground(DependencyObject d, Brush value) => d.SetValue(HeaderButtonsForegroundProperty, value);

    /// <summary>Цвет служебных кнопок в заголовке</summary>
    public static Brush GetHeaderButtonsForeground(DependencyObject d) => (Brush) d.GetValue(HeaderButtonsForegroundProperty);

    #endregion  

}