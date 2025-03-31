using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace WPR.Helpers;

public static class ButtonHelper
{

    #region Запретить Ripple выходить за границы кнопки

    /// <summary>
    /// Запретить Ripple выходить за границы кнопки
    /// </summary>
    public static bool GetNoRippleCircleOutside(DependencyObject obj) => (bool)obj.GetValue(NoRippleCircleOutsideProperty);

    public static void SetNoRippleCircleOutside(DependencyObject obj, bool value) => obj.SetValue(NoRippleCircleOutsideProperty, value);

    public static readonly DependencyProperty NoRippleCircleOutsideProperty =
        DependencyProperty.RegisterAttached("NoRippleCircleOutside", typeof(bool), typeof(ButtonHelper), new PropertyMetadata(true));
    #endregion

}