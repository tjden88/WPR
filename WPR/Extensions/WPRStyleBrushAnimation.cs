using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPR.Extensions;

/// <summary>
/// Анимация кистей на основе цветов текущего стиля
/// </summary>
public class WPRStyleBrushAnimation : AnimationTimeline
{
    public override Type TargetPropertyType => typeof(Brush);

    public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock) =>
        GetCurrentValue(defaultOriginValue as Brush,
            defaultDestinationValue as Brush,
            animationClock);

    public object GetCurrentValue(Brush defaultOriginValue, Brush defaultDestinationValue, AnimationClock animationClock)
    {
        if (animationClock?.CurrentProgress == null)
            return Brushes.Transparent;

        //use the standard values if From and To are not set 
        //(it is the value of the given property)
        defaultOriginValue = FromBrush ?? defaultOriginValue;
        defaultDestinationValue = ToBrush ?? defaultDestinationValue;

        return animationClock.CurrentProgress.Value switch
        {
            0 => defaultOriginValue,
            1 => defaultDestinationValue,
            _ => new VisualBrush(new Border()
            {
                Width = 1,
                Height = 1,
                Background = defaultOriginValue,
                Child = new Border()
                {
                    Background = defaultDestinationValue,
                    Opacity = animationClock.CurrentProgress.Value,
                }
            })
        };
    }

    protected override Freezable CreateInstanceCore() => this;

    private Brush ToBrush => Design.GetBrushFromResource(To);
    private Brush FromBrush => Design.GetBrushFromResource(From);


    #region To : Design.StyleBrushes - Кисть назанчения

    /// <summary>Кисть назанчения</summary>
    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(
            nameof(To),
            typeof(Design.StyleBrushes),
            typeof(WPRStyleBrushAnimation),
            new PropertyMetadata(default(Design.StyleBrushes)));

    /// <summary>Кисть назанчения</summary>
    [Category("WPRStyleBrushAnimation")]
    [Description("Кисть назанчения")]
    public Design.StyleBrushes To
    {
        get => (Design.StyleBrushes) GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    #endregion


    #region From : Design.StyleBrushes - Начальная кисть

    /// <summary>Начальная кисть</summary>
    public static readonly DependencyProperty FromProperty =
        DependencyProperty.Register(
            nameof(From),
            typeof(Design.StyleBrushes),
            typeof(WPRStyleBrushAnimation),
            new PropertyMetadata(default(Design.StyleBrushes)));

    /// <summary>Начальная кисть</summary>
    [Category("WPRStyleBrushAnimation")]
    [Description("Начальная кисть")]
    public Design.StyleBrushes From
    {
        get => (Design.StyleBrushes) GetValue(FromProperty);
        set => SetValue(FromProperty, value);
    }

    #endregion

}