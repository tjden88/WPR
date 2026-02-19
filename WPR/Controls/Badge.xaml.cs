using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace WPR;

/// <summary>Наклейка с контентом на элемент</summary>
public class Badge : ContentControl
{

    private readonly DispatcherTimer _timer = new(DispatcherPriority.Background);

    static Badge()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Badge), new FrameworkPropertyMetadata(typeof(Badge)));
    }

    public Badge()
    {
        IsVisibleChanged += OnIsVisibleChanged;
        _timer.Tick += (_, _) => AnimateBadge();
        UpdatePulse();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => UpdatePulse(); // запустить таймер если надо
    private void OnUnloaded(object sender, RoutedEventArgs e) => _timer.Stop();

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (Visibility == Visibility.Visible)
            Dispatcher.BeginInvoke(new Action(AnimateBadge), DispatcherPriority.Background);

        UpdatePulse();
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        AnimateBadge();
    }

    #region BadgeVisible : bool - Видимость бейджа

    /// <summary>Видимость бейджа</summary>
    public static readonly DependencyProperty BadgeVisibleProperty =
        DependencyProperty.Register(
            nameof(BadgeVisible),
            typeof(bool),
            typeof(Badge),
            new PropertyMetadata(true));

    /// <summary>Видимость бейджа</summary>
    [Category("Badge")]
    [Description("Видимость бейджа")]
    public bool BadgeVisible
    {
        get => (bool)GetValue(BadgeVisibleProperty);
        set => SetValue(BadgeVisibleProperty, value);
    }

    #endregion

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
        get => (Thickness)GetValue(BadgeMarginProperty);
        set => SetValue(BadgeMarginProperty, value);
    }

    #endregion

    #region PulsePeriod : TimeSpan - Период пульсации бейджа, когда он видим

    /// <summary>Период пульсации бейджа, когда он видим</summary>
    public static readonly DependencyProperty PulsePeriodProperty =
        DependencyProperty.Register(
            nameof(PulsePeriod),
            typeof(TimeSpan),
            typeof(Badge),
            new PropertyMetadata(TimeSpan.Zero, OnPulsePeriodChanged));

    private static void OnPulsePeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var badge = (Badge)d;
        badge._timer.Interval = (TimeSpan)e.NewValue;
        badge.UpdatePulse();
    }

    /// <summary>Период пульсации бейджа, когда он видим</summary>
    [Category("Badge")]
    [Description("Период пульсации бейджа, когда он видим")]
    public TimeSpan PulsePeriod
    {
        get => (TimeSpan)GetValue(PulsePeriodProperty);
        set => SetValue(PulsePeriodProperty, value);
    }

    #endregion

    private void UpdatePulse()
    {
        if (Visibility != Visibility.Visible || _timer.Interval <= TimeSpan.Zero)
            _timer.Stop();
        else
            _timer.Start();
    }

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