using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPR;

/// <summary>Наклейка с контентом на элемент</summary>
public class Badge : ContentControl
{
    static Badge()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Badge), new FrameworkPropertyMetadata(typeof(Badge)));
    }

    /// <summary>Контент бейджа</summary>
    public static readonly DependencyProperty BageContentProperty = DependencyProperty.Register(
        nameof(BageContent), typeof(object), typeof(Badge),
        new PropertyMetadata(string.Empty, OnContentChanged));

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!Equals(e.NewValue, e.OldValue))
            ((Badge) d).AnimateBage();
    }

    public object BageContent
    {
        get => GetValue(BageContentProperty);
        set => SetValue(BageContentProperty, value);
    }

    /// <summary> Видимость бейджа </summary>
    public static readonly DependencyProperty BageVisibleProperty = DependencyProperty.Register(nameof(BageVisible), typeof(bool), typeof(Badge), 
        new PropertyMetadata(false, BadgeVisibleOnChanged));

    private static void BadgeVisibleOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!Equals(e.NewValue, e.OldValue) && (bool)e.NewValue)
            ((Badge) d).AnimateBage();
    }

    public bool BageVisible
    {
        get => (bool)GetValue(BageVisibleProperty);
        set => SetValue(BageVisibleProperty, value);
    }


    #region BageMargin : Thickness - Положение бейджа

    /// <summary>Положение бейджа</summary>
    public static readonly DependencyProperty BageMarginProperty =
        DependencyProperty.Register(
            nameof(BageMargin),
            typeof(Thickness),
            typeof(Badge),
            new PropertyMetadata(default(Thickness)));

    /// <summary>Положение бейджа</summary>
    [Category("Badge")]
    [Description("Положение бейджа")]
    public Thickness BageMargin
    {
        get => (Thickness) GetValue(BageMarginProperty);
        set => SetValue(BageMarginProperty, value);
    }

    #endregion


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