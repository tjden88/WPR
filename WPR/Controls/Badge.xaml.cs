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

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        if (!Equals(oldContent, newContent))
            AnimateBadge();
    }

    /// <summary> Видимость бейджа </summary>
    public static readonly DependencyProperty BadgeVisibleProperty = DependencyProperty.Register(nameof(BadgeVisible), typeof(bool), typeof(Badge), 
        new PropertyMetadata(true, BadgeVisibleOnChanged));

    private static void BadgeVisibleOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!Equals(e.NewValue, e.OldValue) && (bool)e.NewValue)
            ((Badge) d).AnimateBadge();
    }

    public bool BadgeVisible
    {
        get => (bool)GetValue(BadgeVisibleProperty);
        set => SetValue(BadgeVisibleProperty, value);
    }


    #region BadgeMargin : Thickness - Положение бейджа

    /// <summary>Положение бейджа</summary>
    public static readonly DependencyProperty BadgeMarginProperty =
        DependencyProperty.Register(
            nameof(BadgeMargin),
            typeof(Thickness),
            typeof(Badge),
            new PropertyMetadata(default(Thickness)));

    /// <summary>Положение бейджа</summary>
    [Category("Badge")]
    [Description("Положение бейджа")]
    public Thickness BadgeMargin
    {
        get => (Thickness) GetValue(BadgeMarginProperty);
        set => SetValue(BadgeMarginProperty, value);
    }

    #endregion


    private void AnimateBadge()
    {
        if (GetTemplateChild("BadgeBorder") is not Border border) return;
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