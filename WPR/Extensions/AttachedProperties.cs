using System.Windows;

namespace WPR.Extensions;

internal static class AttachedProperties
{

    #region Запретить Ripple выходить за границы кнопки

    /// <summary>
    /// Запретить Ripple выходить за границы кнопки
    /// </summary>
    internal static bool GetNoRippleCircleOutside(DependencyObject obj) => (bool)obj.GetValue(NoRippleCircleOutsideProperty);

    internal static void SetNoRippleCircleOutside(DependencyObject obj, bool value) => obj.SetValue(NoRippleCircleOutsideProperty, value);

    internal static readonly DependencyProperty NoRippleCircleOutsideProperty =
        DependencyProperty.RegisterAttached("NoRippleCircleOutside", typeof(bool), typeof(AttachedProperties), new PropertyMetadata(true));
    #endregion


    #region StayRippleOnCenter

    /// <summary>
    /// Анимация всегда будет начинаться из центра     
    /// </summary>
    internal static readonly DependencyProperty RippleOnCenterProperty = DependencyProperty.RegisterAttached(
        "RippleOnCenter", typeof(bool), typeof(AttachedProperties), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    internal static void SetRippleOnCenter(DependencyObject element, bool value)
    {
        element?.SetValue(RippleOnCenterProperty, value);
    }

    internal static bool GetRippleOnCenter(DependencyObject element) => (bool)element.GetValue(RippleOnCenterProperty);

    #endregion


    #region IsRippleEnabled

    /// <summary>
    /// Выключает анимацию
    /// </summary>
    internal static readonly DependencyProperty IsRippleEnabledProperty = DependencyProperty.RegisterAttached(
        "IsRippleEnabled", typeof(bool), typeof(AttachedProperties), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

    internal static void SetIsRippleEnabled(DependencyObject element, bool value)
    {
        element?.SetValue(IsRippleEnabledProperty, value);
    }

    internal static bool GetIsRippleEnabled(DependencyObject element)
    {
        return (bool)element.GetValue(IsRippleEnabledProperty);
    }

    #endregion

}