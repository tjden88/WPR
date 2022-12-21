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

    public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
    {
        return GetCurrentValue(defaultOriginValue as Brush,
            defaultDestinationValue as Brush,
            animationClock);
    }
    public object GetCurrentValue(Brush defaultOriginValue, Brush defaultDestinationValue, AnimationClock animationClock)
    {
        if (animationClock?.CurrentProgress == null)
            return Brushes.Transparent;

        //use the standard values if From and To are not set 
        //(it is the value of the given property)
        defaultOriginValue = _From ?? defaultOriginValue;
        defaultDestinationValue = _To ?? defaultDestinationValue;

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

    protected override Freezable CreateInstanceCore() => new BrushAnimation();

    private Brush _To => Design.GetBrushFromResource(To);
    private Brush _From => Design.GetBrushFromResource(From);


    #region To : Design.StyleBrush - Кисть назанчения

    /// <summary>Кисть назанчения</summary>
    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(
            nameof(To),
            typeof(Design.StyleBrush),
            typeof(WPRStyleBrushAnimation),
            new PropertyMetadata(default(Design.StyleBrush)));

    /// <summary>Кисть назанчения</summary>
    [Category("WPRStyleBrushAnimation")]
    [Description("Кисть назанчения")]
    public Design.StyleBrush To
    {
        get => (Design.StyleBrush) GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    #endregion


    #region From : Design.StyleBrush - Начальная кисть

    /// <summary>Начальная кисть</summary>
    public static readonly DependencyProperty FromProperty =
        DependencyProperty.Register(
            nameof(From),
            typeof(Design.StyleBrush),
            typeof(WPRStyleBrushAnimation),
            new PropertyMetadata(default(Design.StyleBrush)));

    /// <summary>Начальная кисть</summary>
    [Category("WPRStyleBrushAnimation")]
    [Description("Начальная кисть")]
    public Design.StyleBrush From
    {
        get => (Design.StyleBrush) GetValue(FromProperty);
        set => SetValue(FromProperty, value);
    }

    #endregion

}