using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPR.Extensions;

public class BrushAnimation : AnimationTimeline
{
    public override Type TargetPropertyType => typeof(Brush);

    public override object GetCurrentValue(object defaultOriginValue,
        object defaultDestinationValue,
        AnimationClock animationClock)
    {
        return GetCurrentValue(defaultOriginValue as Brush,
            defaultDestinationValue as Brush,
            animationClock);
    }
    public object GetCurrentValue(Brush defaultOriginValue,
        Brush defaultDestinationValue,
        AnimationClock animationClock)
    {
        if (animationClock?.CurrentProgress == null)
            return Brushes.Transparent;

        //use the standard values if From and To are not set 
        //(it is the value of the given property)
        defaultOriginValue = From ?? defaultOriginValue;
        defaultDestinationValue = To ?? defaultDestinationValue;

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
                    Background = defaultDestinationValue, Opacity = animationClock.CurrentProgress.Value,
                }
            })
        };
    }

    protected override Freezable CreateInstanceCore() => this;

    #region To : Brush - Конечная кисть

    /// <summary>Конечная кисть</summary>
    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(
            nameof(To),
            typeof(Brush),
            typeof(BrushAnimation),
            new PropertyMetadata(default(Brush)));

    /// <summary>Конечная кисть</summary>
    [Description("Конечная кисть")]
    public Brush To
    {
        get => (Brush) GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    #endregion

    #region From : Brush - Начальная кисть

    /// <summary>Начальная кисть</summary>
    public static readonly DependencyProperty FromProperty =
        DependencyProperty.Register(
            nameof(From),
            typeof(Brush),
            typeof(BrushAnimation),
            new PropertyMetadata(default(Brush)));

    /// <summary>Начальная кисть</summary>
    //[Category("")]
    [Description("Начальная кисть")]
    public Brush From
    {
        get => (Brush) GetValue(FromProperty);
        set => SetValue(FromProperty, value);
    }

    #endregion  

}