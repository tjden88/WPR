using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPR.Controls
{
    /// <summary>Наклейка с контентом на элемент</summary>
    public class Bage : ContentControl
    {
        static Bage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Bage), new FrameworkPropertyMetadata(typeof(Bage)));
        }

        /// <summary>Контент бейджа</summary>
        public static readonly DependencyProperty BageContentProperty = DependencyProperty.Register("BageContent", typeof(object), typeof(Bage),
            new PropertyMetadata(string.Empty));

        public object BageContent
        {
            get => GetValue(BageContentProperty);
            set
            {
                SetValue(BageContentProperty, value);
                AnimateBage();
            }
        }

        /// <summary> Видимость бейджа </summary>
        public static readonly DependencyProperty BageVisibleProperty = DependencyProperty.Register("BageVisible", typeof(Visibility), typeof(Bage), 
            new PropertyMetadata(Visibility.Hidden));

        public Visibility BageVisible
        {
            get => (Visibility)GetValue(BageVisibleProperty);
            set
            {
                SetValue(BageVisibleProperty, value);
                if (value== Visibility.Visible) AnimateBage();
            }
        }


        private void AnimateBage ()
        {
            if (GetTemplateChild("BageBorder") is not Border border) return;
            ScaleTransform scaleTransform = new(1.7, 1.7, border.ActualWidth / 2, border.ActualHeight / 2);
            border.RenderTransform = scaleTransform;
            DoubleAnimation doubleAnimation = new()
            {
                Duration = TimeSpan.FromSeconds(0.3),
                DecelerationRatio = 0.5,
                To = 1
            };
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);
        }
    }
}
