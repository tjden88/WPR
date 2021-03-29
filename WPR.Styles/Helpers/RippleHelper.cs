using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WPR.Styles.Helpers
{
    public static class RippleHelper
    {
        #region Цвет анимации при наведении на кнопку

        /// <summary>
        /// Цвет анимации при наведении на кнопку
        /// </summary>
        public static SolidColorBrush GetFeedbackBrush(DependencyObject obj) => (SolidColorBrush)obj?.GetValue(FeedbackBrushProperty);

        public static void SetFeedbackBrush(DependencyObject obj, SolidColorBrush value) => obj?.SetValue(FeedbackBrushProperty, value);

        // Using a DependencyProperty as the backing store for FeedbackBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FeedbackBrushProperty =
            DependencyProperty.RegisterAttached("FeedbackBrush", typeof(SolidColorBrush), typeof(RippleHelper), new PropertyMetadata());
        #endregion

        //#region Прозрачность цвета кнопки при наведении
        ///// <summary>
        ///// Прозрачность цвета кнопки при наведении
        ///// </summary>
        //public static double GetAnimationOpacity(DependencyObject obj)
        //{
        //    return (double)obj?.GetValue(AnimationOpacityProperty);
        //}

        //public static void SetAnimationOpacity(DependencyObject obj, double value)
        //{
        //    obj?.SetValue(AnimationOpacityProperty, value);
        //}

        //public static readonly DependencyProperty AnimationOpacityProperty =
        //    DependencyProperty.RegisterAttached("AnimationOpacity", typeof(double), typeof(RippleHelper), new PropertyMetadata(0.4));
        //#endregion

        #region StayOnCenter

        /// <summary>
        /// Анимация всегда будет начинаться из центра     
        /// </summary>
        public static readonly DependencyProperty IsCenteredProperty = DependencyProperty.RegisterAttached(
            "IsCentered", typeof(bool), typeof(RippleHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetIsCentered(DependencyObject element, bool value)
        {
            element?.SetValue(IsCenteredProperty, value);
        }

        public static bool GetIsCentered(DependencyObject element)
        {
            return (bool)element?.GetValue(IsCenteredProperty);
        }

        #endregion

        #region IsEnabled

        /// <summary>
        /// Выключает анимацию
        /// </summary>
        public static readonly DependencyProperty IsRippleEnabledProperty = DependencyProperty.RegisterAttached(
            "IsRippleEnabled", typeof(bool), typeof(RippleHelper), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetIsRippleEnabled(DependencyObject element, bool value)
        {
            element?.SetValue(IsRippleEnabledProperty, value);
        }

        public static bool GetIsRippleEnabled(DependencyObject element)
        {
            return (bool)element?.GetValue(IsRippleEnabledProperty);
        }

        #endregion
    }
}
