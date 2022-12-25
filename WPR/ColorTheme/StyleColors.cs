﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace WPR.ColorTheme;

/// <summary>
/// Свойства - цвета текущей схемы
/// </summary>
public class StyleColors : DependencyObject
{
    /// <summary>
    /// Коллекция кистей цветовой темы
    /// </summary>
    public enum StyleBrushes
    {
        None,
        Transparent,
        PrimaryColorBrush,
        DarkPrimaryColorBrush,
        LightPrimaryColorBrush,
        AccentColorBrush,
        TextColorBrush,
        SecondaryColorBrush,
        DividerColorBrush,
        BackgroundColorBrush,
        SecondaryBackgroundColorBrush,
        WindowBackgroundColorBrush,
        LightWindowBackgroundColorBrush,
        DarkWindowBackgroundColorBrush,
        InactiveWindowBackgroundColor,
        WindowForegroundColor,
        AnimationEnterColorBrush,
        DangerColorBrush,
        WhiteBrush,
        DarkBrush
    }


    // Цвета по умолчанию для светлой темы
    private static readonly Color _LightColor = Colors.White;
    private static readonly Color _DarkColor = (Color)ColorConverter.ConvertFromString("#1C1C1C")!;
    private static readonly Color _PrimaryColor = (Color)ColorConverter.ConvertFromString("#3F51B5")!;
    private static readonly Color _DarkPrimaryColor = (Color)ColorConverter.ConvertFromString("#FF212E80")!;
    private static readonly Color _LightPrimaryColor = (Color)ColorConverter.ConvertFromString("#C5CAE9")!;
    private static readonly Color _SecondaryColor = (Color)ColorConverter.ConvertFromString("#FF8B8B8B")!;
    private static readonly Color _AccentColor = (Color)ColorConverter.ConvertFromString("#FF5722")!;
    private static readonly Color _DangerColor = (Color)ColorConverter.ConvertFromString("#CA0B00")!;
    private static readonly Color _TextColor = (Color)ColorConverter.ConvertFromString("#FF383838")!;
    private static readonly Color _DividerColor = (Color)ColorConverter.ConvertFromString("#FFE0E0E0")!;
    private static readonly Color _ShadowColor = Colors.DimGray;

    private static readonly Color _BackgroundColor = _LightColor;
    private static readonly Color _WindowBackgroundColor = _PrimaryColor;
    private static readonly Color _InactiveWindowBackgroundColor = (Color)ColorConverter.ConvertFromString("#FF9FA8DA")!;
    private static readonly Color _WindowForegroundColor = _BackgroundColor;

    private static readonly Color _SecondaryBackgroundColor = _LightColor;
    private static readonly Color _LightWindowBackgroundColor = _PrimaryColor;
    private static readonly Color _DarkWindowBackgroundColor = _DarkColor;


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
        get => (Color)GetValue(ShadowColorProperty);
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
        get => (Color)GetValue(LightColorProperty);
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
        get => (Color)GetValue(DarkColorProperty);
        set => SetValue(DarkColorProperty, value);
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
            new PropertyMetadata(_TextColor));

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
            new PropertyMetadata(_DividerColor));

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
            new PropertyMetadata(_BackgroundColor));

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
            new PropertyMetadata(_SecondaryBackgroundColor));

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
            new PropertyMetadata(_WindowBackgroundColor));

    /// <summary>Текущий фон окна</summary>
    [Category("StyleColors")]
    [Description("Текущий фон окна")]
    public Color WindowBackgroundColor
    {
        get => (Color) GetValue(WindowBackgroundColorProperty);
        set => SetValue(WindowBackgroundColorProperty, value);
    }

    #endregion


    #region LightWindowBackgroundColor : Color - Фон окна светлой темы

    /// <summary>Фон окна светлой темы</summary>
    public static readonly DependencyProperty LightWindowBackgroundColorProperty =
        DependencyProperty.Register(
            nameof(LightWindowBackgroundColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_LightWindowBackgroundColor));

    /// <summary>Фон окна светлой темы</summary>
    [Category("StyleColors")]
    [Description("Фон окна светлой темы")]
    public Color LightWindowBackgroundColor
    {
        get => (Color)GetValue(LightWindowBackgroundColorProperty);
        set => SetValue(LightWindowBackgroundColorProperty, value);
    }

    #endregion


    #region DarkWindowBackgroundColor : Color - Фон окна тёмной темы

    /// <summary>Фон окна тёмной темы</summary>
    public static readonly DependencyProperty DarkWindowBackgroundColorProperty =
        DependencyProperty.Register(
            nameof(DarkWindowBackgroundColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_DarkWindowBackgroundColor));

    /// <summary>Фон окна тёмной темы</summary>
    [Category("StyleColors")]
    [Description("Фон окна тёмной темы")]
    public Color DarkWindowBackgroundColor
    {
        get => (Color)GetValue(DarkWindowBackgroundColorProperty);
        set => SetValue(DarkWindowBackgroundColorProperty, value);
    }

    #endregion


    #region InactiveWindowBackgroundColor : Color - Цвет фона неактивного окна

    /// <summary>Цвет фона неактивного окна</summary>
    public static readonly DependencyProperty InactiveWindowBackgroundColorProperty =
        DependencyProperty.Register(
            nameof(InactiveWindowBackgroundColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_InactiveWindowBackgroundColor));

    /// <summary>Цвет фона неактивного окна</summary>
    [Category("StyleColors")]
    [Description("Цвет фона неактивного окна")]
    public Color InactiveWindowBackgroundColor
    {
        get => (Color)GetValue(InactiveWindowBackgroundColorProperty);
        set => SetValue(InactiveWindowBackgroundColorProperty, value);
    }

    #endregion


    #region WindowForegroundColor : Color - Цвет текста заголовка окна

    /// <summary>Цвет текста заголовка окна</summary>
    public static readonly DependencyProperty WindowForegroundColorProperty =
        DependencyProperty.Register(
            nameof(WindowForegroundColor),
            typeof(Color),
            typeof(StyleColors),
            new PropertyMetadata(_WindowForegroundColor));

    /// <summary>Цвет текста заголовка окна</summary>
    [Category("StyleColors")]
    [Description("Цвет текста заголовка окна")]
    public Color WindowForegroundColor
    {
        get => (Color) GetValue(WindowForegroundColorProperty);
        set => SetValue(WindowForegroundColorProperty, value);
    }

    #endregion
}