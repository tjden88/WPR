using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPR.Controls;

/// <summary>Наклейка с контентом на элемент</summary>
public class WPRBage : ContentControl
{
    static WPRBage()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRBage), new FrameworkPropertyMetadata(typeof(WPRBage)));
    }

    /// <summary>Контент бейджа</summary>
    public static readonly DependencyProperty BageContentProperty = DependencyProperty.Register(
        "BageContent", typeof(object), typeof(WPRBage),
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
    public static readonly DependencyProperty BageVisibleProperty = DependencyProperty.Register("BageVisible", typeof(bool), typeof(WPRBage), 
        new PropertyMetadata(false));

    public bool BageVisible
    {
        get => (bool)GetValue(BageVisibleProperty);
        set
        {
            SetValue(BageVisibleProperty, value);
            if (value) AnimateBage();
        }
    }


    private void AnimateBage()
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