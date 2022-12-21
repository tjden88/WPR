using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace WPR.Extensions;

/// <summary>
/// Анимация радиусов
/// </summary>
public class WPRCornerRadiusAnimation : AnimationTimeline
{
    protected override Freezable CreateInstanceCore() => this;

    public override Type TargetPropertyType => typeof(CornerRadius);

    public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock) =>
        GetCurrentValue(defaultOriginValue is CornerRadius value ? value : default,
            defaultDestinationValue is CornerRadius radius ? radius : default,
            animationClock);

    public object GetCurrentValue(CornerRadius defaultOriginValue, CornerRadius defaultDestinationValue, AnimationClock animationClock)
    {
        if (animationClock?.CurrentProgress == null)
            return new CornerRadius();

        defaultOriginValue = From == _NanRadius ? defaultOriginValue : From;
        defaultDestinationValue = To == _NanRadius ? defaultDestinationValue : To;

        double CalcRadius(double orig, double dest)
        {
            return orig + animationClock.CurrentProgress.Value * (dest - orig);

        }

        return animationClock.CurrentProgress.Value switch
        {
            0 => defaultOriginValue,
            1 => defaultDestinationValue,
            _ => new CornerRadius
            {
                BottomLeft = CalcRadius(defaultOriginValue.BottomLeft, defaultDestinationValue.BottomLeft),
                BottomRight = CalcRadius(defaultOriginValue.BottomRight, defaultDestinationValue.BottomRight),
                TopLeft = CalcRadius(defaultOriginValue.TopLeft, defaultDestinationValue.TopLeft),
                TopRight = CalcRadius(defaultOriginValue.TopRight, defaultDestinationValue.TopRight),
            }

        };
    }


    private static readonly CornerRadius _NanRadius = new(double.NaN);

    #region From : CornerRadius - Начальное значение радиуса

    /// <summary>Начальное значение радиуса</summary>
    public static readonly DependencyProperty FromProperty =
        DependencyProperty.Register(
            nameof(From),
            typeof(CornerRadius),
            typeof(WPRCornerRadiusAnimation),
            new PropertyMetadata(_NanRadius));

    /// <summary>Начальное значение радиуса</summary>
    [Category("WPRCornerRadiusAnimation")]
    [Description("Начальное значение радиуса")]
    public CornerRadius From
    {
        get => (CornerRadius)GetValue(FromProperty);
        set => SetValue(FromProperty, value);
    }

    #endregion


    #region To : CornerRadius - Конечное значение радиуса

    /// <summary>Конечное значение радиуса</summary>
    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(
            nameof(To),
            typeof(CornerRadius),
            typeof(WPRCornerRadiusAnimation),
            new PropertyMetadata(_NanRadius));

    /// <summary>Конечное значение радиуса</summary>
    [Category("WPRCornerRadiusAnimation")]
    [Description("Конечное значение радиуса")]
    public CornerRadius To
    {
        get => (CornerRadius)GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    #endregion
}