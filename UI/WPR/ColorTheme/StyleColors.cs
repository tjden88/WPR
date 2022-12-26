using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
// ReSharper disable InconsistentNaming

namespace WPR.ColorTheme;

/// <summary>
/// Свойства - цвета текущей схемы
/// </summary>
public class StyleColors : DependencyObject
{
    // Цвета по умолчанию
    internal static readonly Color _LightColor = Colors.White;
    internal static readonly Color _DarkColor = (Color)ColorConverter.ConvertFromString("#1C1C1C")!;

    internal static readonly Color _PrimaryColor = (Color)ColorConverter.ConvertFromString("#3F51B5")!;
    internal static readonly Color _DarkPrimaryColor = (Color)ColorConverter.ConvertFromString("#FF212E80")!;
    internal static readonly Color _LightPrimaryColor = (Color)ColorConverter.ConvertFromString("#FF9FA8DA")!;
    internal static readonly Color _SecondaryColor = (Color)ColorConverter.ConvertFromString("#FF8B8B8B")!;
    internal static readonly Color _AccentColor = (Color)ColorConverter.ConvertFromString("#FF5722")!;
    internal static readonly Color _DangerColor = (Color)ColorConverter.ConvertFromString("#CA0B00")!;


    // Цвета по умолчанию для светлой темы
    internal static readonly Color _LightTextColor = _DarkColor;
    internal static readonly Color _LightDividerColor = (Color)ColorConverter.ConvertFromString("#FFE0E0E0")!;
    internal static readonly Color _LightShadowColor = Colors.DimGray;
    internal static readonly Color _LightBackgroundColor = _LightColor;
    internal static readonly Color _LightSecondaryBackgroundColor = Colors.WhiteSmoke;
    internal static readonly Color _LightWindowBackgroundColor = _PrimaryColor;


    // Цвета по умолчанию для тёмной темы
    internal static readonly Color _DarkTextColor = _LightColor;
    internal static readonly Color _DarkDividerColor = (Color)ColorConverter.ConvertFromString("#FF494949")!;
    internal static readonly Color _DarkShadowColor = Colors.Black;
    internal static readonly Color _DarkBackgroundColor = _DarkColor;
    internal static readonly Color _DarkSecondaryBackgroundColor = (Color)ColorConverter.ConvertFromString("#FF323232")!;
    internal static readonly Color _DarkWindowBackgroundColor = _DarkColor;


    #region ShadowColor : Color - Цвет тени

    /// <summary>Цвет тени</summary>
    public static readonly DependencyProperty ShadowColorProperty =
        DependencyProperty.Register(
            nameof(ShadowColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_LightShadowColor));

    /// <summary>Цвет тени</summary>
    [Category("StyleColors")]
    [Description("Цвет тени")]
    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    #endregion


    #region PrimaryColor : Color - Основной цвет

    /// <summary>Основной цвет</summary>
    public static readonly DependencyProperty PrimaryColorProperty =
        DependencyProperty.Register(
            nameof(PrimaryColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_PrimaryColor));

    /// <summary>Основной цвет</summary>
    [Category("StyleColors")]
    [Description("Основной цвет")]
    public Color PrimaryColor
    {
        get => (Color)GetValue(PrimaryColorProperty);
        set => SetValue(PrimaryColorProperty, value);
    }

    #endregion


    #region DarkPrimaryColor : Color - Тёмный основной цвет

    /// <summary>Тёмный основной цвет</summary>
    public static readonly DependencyProperty DarkPrimaryColorProperty =
        DependencyProperty.Register(
            nameof(DarkPrimaryColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_DarkPrimaryColor));

    /// <summary>Тёмный основной цвет</summary>
    [Category("StyleColors")]
    [Description("Тёмный основной цвет")]
    public Color DarkPrimaryColor
    {
        get => (Color)GetValue(DarkPrimaryColorProperty);
        set => SetValue(DarkPrimaryColorProperty, value);
    }

    #endregion


    #region LightPrimaryColor : Color - Светлый основной цвет

    /// <summary>Светлый основной цвет</summary>
    public static readonly DependencyProperty LightPrimaryColorProperty =
        DependencyProperty.Register(
            nameof(LightPrimaryColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_LightPrimaryColor));

    /// <summary>Светлый основной цвет</summary>
    [Category("StyleColors")]
    [Description("Светлый основной цвет")]
    public Color LightPrimaryColor
    {
        get => (Color)GetValue(LightPrimaryColorProperty);
        set => SetValue(LightPrimaryColorProperty, value);
    }

    #endregion


    #region SecondaryColor : Color - Вспомогательный цвет

    /// <summary>Вспомогательный цвет</summary>
    public static readonly DependencyProperty SecondaryColorProperty =
        DependencyProperty.Register(
            nameof(SecondaryColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_SecondaryColor));

    /// <summary>Вспомогательный цвет</summary>
    [Category("StyleColors")]
    [Description("Вспомогательный цвет")]
    public Color SecondaryColor
    {
        get => (Color)GetValue(SecondaryColorProperty);
        set => SetValue(SecondaryColorProperty, value);
    }

    #endregion


    #region AccentColor : Color - Цвет акцента

    /// <summary>Цвет акцента</summary>
    public static readonly DependencyProperty AccentColorProperty =
        DependencyProperty.Register(
            nameof(AccentColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_AccentColor));

    /// <summary>Цвет акцента</summary>
    [Category("StyleColors")]
    [Description("Цвет акцента")]
    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    #endregion


    #region DangerColor : Color - Цвет предупреждения

    /// <summary>Цвет предупреждения</summary>
    public static readonly DependencyProperty DangerColorProperty =
        DependencyProperty.Register(
            nameof(DangerColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_DangerColor));

    /// <summary>Цвет предупреждения</summary>
    [Category("StyleColors")]
    [Description("Цвет предупреждения")]
    public Color DangerColor
    {
        get => (Color)GetValue(DangerColorProperty);
        set => SetValue(DangerColorProperty, value);
    }

    #endregion


    #region TextColor : Color - Цвет текста

    /// <summary>Цвет текста</summary>
    public static readonly DependencyProperty TextColorProperty =
        DependencyProperty.Register(
            nameof(TextColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_LightTextColor));

    /// <summary>Цвет текста</summary>
    [Category("StyleColors")]
    [Description("Цвет текста")]
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    #endregion


    #region DividerColor : Color - Цвет разделителей

    /// <summary>Цвет разделителей</summary>
    public static readonly DependencyProperty DividerColorProperty =
        DependencyProperty.Register(
            nameof(DividerColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_LightDividerColor));

    /// <summary>Цвет разделителей</summary>
    [Category("StyleColors")]
    [Description("Цвет разделителей")]
    public Color DividerColor
    {
        get => (Color)GetValue(DividerColorProperty);
        set => SetValue(DividerColorProperty, value);
    }

    #endregion


    #region BackgroundColor : Color - Цвет фона

    /// <summary>Цвет фона</summary>
    public static readonly DependencyProperty BackgroundColorProperty =
        DependencyProperty.Register(
            nameof(BackgroundColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_LightBackgroundColor));

    /// <summary>Цвет фона</summary>
    [Category("StyleColors")]
    [Description("Цвет фона")]
    public Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    #endregion


    #region SecondaryBackgroundColor : Color - Вторичный фон (например, для карточек)

    /// <summary>Вторичный фон (например, для карточек)</summary>
    public static readonly DependencyProperty SecondaryBackgroundColorProperty =
        DependencyProperty.Register(
            nameof(SecondaryBackgroundColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_LightSecondaryBackgroundColor));

    /// <summary>Вторичный фон (например, для карточек)</summary>
    [Category("StyleColors")]
    [Description("Вторичный фон (например, для карточек)")]
    public Color SecondaryBackgroundColor
    {
        get => (Color)GetValue(SecondaryBackgroundColorProperty);
        set => SetValue(SecondaryBackgroundColorProperty, value);
    }

    #endregion


    #region WindowBackgroundColor : Color - Текущий фон окна

    /// <summary>Текущий фон окна</summary>
    public static readonly DependencyProperty WindowBackgroundColorProperty =
        DependencyProperty.Register(
            nameof(WindowBackgroundColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_LightWindowBackgroundColor));

    /// <summary>Текущий фон окна</summary>
    [Category("StyleColors")]
    [Description("Текущий фон окна")]
    public Color WindowBackgroundColor
    {
        get => (Color)GetValue(WindowBackgroundColorProperty);
        set => SetValue(WindowBackgroundColorProperty, value);
    }

    #endregion


    #region InactiveWindowBackgroundColor : Color - Цвет фона неактивного окна

    /// <summary>Цвет фона неактивного окна</summary>
    public static readonly DependencyProperty InactiveWindowBackgroundColorProperty =
        DependencyProperty.Register(
            nameof(InactiveWindowBackgroundColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_LightPrimaryColor));

    /// <summary>Цвет фона неактивного окна</summary>
    [Category("StyleColors")]
    [Description("Цвет фона неактивного окна")]
    public Color InactiveWindowBackgroundColor
    {
        get => (Color)GetValue(InactiveWindowBackgroundColorProperty);
        set => SetValue(InactiveWindowBackgroundColorProperty, value);
    }

    #endregion
}