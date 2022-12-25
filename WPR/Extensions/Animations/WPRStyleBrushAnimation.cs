using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPR.ColorTheme;

namespace WPR.Extensions.Animations;

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
                Background = AnimateToOrigin ? defaultDestinationValue : defaultOriginValue,
                Child = new Border
                {
                    Background = AnimateToOrigin ? defaultOriginValue : defaultDestinationValue,
                    Opacity = AnimateToOrigin ? 1 - animationClock.CurrentProgress.Value : animationClock.CurrentProgress.Value,
                }
            })
        };
    }

    protected override Freezable CreateInstanceCore() => this;

    private Brush ToBrush => StyleHelper.GetBrushFromResource(To);
    private Brush FromBrush => StyleHelper.GetBrushFromResource(From);


    #region To : StyleHelper.StyleBrushes - Кисть назанчения

    /// <summary>Кисть назанчения</summary>
    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(
            nameof(To),
            typeof(StyleColors.StyleBrushes),
            typeof(WPRStyleBrushAnimation),
            new PropertyMetadata(default(StyleColors.StyleBrushes)));

    /// <summary>Кисть назанчения</summary>
    [Category("WPRStyleBrushAnimation")]
    [Description("Кисть назанчения")]
    public StyleColors.StyleBrushes To
    {
        get => (StyleColors.StyleBrushes)GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    #endregion


    #region From : StyleHelper.StyleBrushes - Начальная кисть

    /// <summary>Начальная кисть</summary>
    public static readonly DependencyProperty FromProperty =
        DependencyProperty.Register(
            nameof(From),
            typeof(StyleColors.StyleBrushes),
            typeof(WPRStyleBrushAnimation),
            new PropertyMetadata(default(StyleColors.StyleBrushes)));

    /// <summary>Начальная кисть</summary>
    [Category("WPRStyleBrushAnimation")]
    [Description("Начальная кисть")]
    public StyleColors.StyleBrushes From
    {
        get => (StyleColors.StyleBrushes)GetValue(FromProperty);
        set => SetValue(FromProperty, value);
    }

    #endregion


    #region AnimateToOrigin : bool - Анимировать от кисти назначения до оригинальной (альтернативный режим для прозрачных кистей назначения)

    /// <summary>Анимировать от кисти назначения до оригинальной (альтернативный режим для прозрачных кистей назначения)</summary>
    public static readonly DependencyProperty AnimateToOriginProperty =
        DependencyProperty.Register(
            nameof(AnimateToOrigin),
            typeof(bool),
            typeof(WPRStyleBrushAnimation),
            new PropertyMetadata(default(bool)));

    /// <summary>Анимировать от кисти назначения до оригинальной (альтернативный режим для прозрачных кистей назначения)</summary>
    [Category("WPRStyleBrushAnimation")]
    [Description("Анимировать от кисти назначения до оригинальной (альтернативный режим для прозрачных кистей назначения)")]
    public bool AnimateToOrigin
    {
        get => (bool) GetValue(AnimateToOriginProperty);
        set => SetValue(AnimateToOriginProperty, value);
    }

    #endregion

}