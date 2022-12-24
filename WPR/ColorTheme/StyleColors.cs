using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace WPR.ColorTheme;

/// <summary>
/// Свойства - цвета текущей схемы
/// </summary>
public class StyleColors : DependencyObject
{
    #region ShadowColor : Color - Цвет тени

    /// <summary>Цвет тени</summary>
    public static readonly DependencyProperty ShadowColorProperty =
        DependencyProperty.Register(
            nameof(ShadowColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(Colors.DimGray));

    /// <summary>Цвет тени</summary>
    [Category("StyleColors")]
    [Description("Цвет тени")]
    public Color ShadowColor
    {
        get => (Color) GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    #endregion
}