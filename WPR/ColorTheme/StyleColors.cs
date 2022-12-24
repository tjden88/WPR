using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace WPR.ColorTheme;

/// <summary>
/// Свойства - цвета текущей схемы
/// </summary>
public class StyleColors : DependencyObject
{
    private static readonly Color _ShadowColor = Colors.DimGray;
    private static readonly Color _LightColor = Colors.White;
    private static readonly Color _DarkColor = (Color)ColorConverter.ConvertFromString("#1C1C1C")!;


    #region ShadowColor : Color - Цвет тени

    /// <summary>Цвет тени</summary>
    public static readonly DependencyProperty ShadowColorProperty =
        DependencyProperty.Register(
            nameof(ShadowColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_ShadowColor));

    /// <summary>Цвет тени</summary>
    [Category("StyleColors")]
    [Description("Цвет тени")]
    public Color ShadowColor
    {
        get => (Color) GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    #endregion


    #region LightColor : Color - Цвет светлой схемы

    /// <summary>Цвет светлой схемы</summary>
    public static readonly DependencyProperty LightColorProperty =
        DependencyProperty.Register(
            nameof(LightColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_LightColor));

    /// <summary>Цвет светлой схемы</summary>
    [Category("StyleColors")]
    [Description("Цвет светлой схемы")]
    public Color LightColor
    {
        get => (Color) GetValue(LightColorProperty);
        set => SetValue(LightColorProperty, value);
    }

    #endregion


    #region DarkColor : Color - Цвет тёмной темы

    /// <summary>Цвет тёмной темы</summary>
    public static readonly DependencyProperty DarkColorProperty =
        DependencyProperty.Register(
            nameof(DarkColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_DarkColor));

    /// <summary>Цвет тёмной темы</summary>
    [Category("StyleColors")]
    [Description("Цвет тёмной темы")]
    public Color DarkColor
    {
        get => (Color) GetValue(DarkColorProperty);
        set => SetValue(DarkColorProperty, value);
    }

    #endregion
}