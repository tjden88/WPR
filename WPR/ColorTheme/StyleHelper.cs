using System;
using System.Windows;
using System.Windows.Media;
using WPR.MVVM.Converters;
using WPR.MVVM.Converters.Base;

namespace WPR.ColorTheme;

/// <summary>
/// Установка и изменение цветовой темы
/// </summary>
public static class StyleHelper
{
    private static readonly TypeConverter<SolidColorBrush> _BrushLightOrDarkConverter = new(new BrushLightOrDarkConverter());

    private static readonly StyleColors _StyleColors = (StyleColors)Application.Current.Resources["StyleColors"];
    private static Color DarkColor => _StyleColors.DarkColor; // Кисть тёмной темы
    private static Color WhiteColor => _StyleColors.LightColor; // Кисть светлой темы


    /// <summary> Происходит при любом изменении цветовой схемы</summary>
    public static event EventHandler StyleChanged;


    /// <summary>Установлена ли тёмная тема</summary>
    public static bool IsDarkTheme => _StyleColors.DarkColor == _StyleColors.BackgroundColor;


    /// <summary>Задать новый рандомный стиль (цветовую палитру) элементам управления</summary>
    public static void SetNewRandomStyle()
    {
        Random rnd = new();
        var rndColor = Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
        SetPrimaryColor(rndColor);
        SetAccentColor(Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
    }


    /// <summary>Установить главный цвет (включая тёмный и светлый)</summary>
    public static void SetPrimaryColor(Color color)
    {
        if (_StyleColors.LightWindowBackgroundColor == _StyleColors.PrimaryColor)
            _StyleColors.LightWindowBackgroundColor = color;

        _StyleColors.PrimaryColor = color;
        _StyleColors.DarkPrimaryColor = Darken(color, 1.2);
        _StyleColors.LightPrimaryColor = Lighten(color, 1.5);

        SetWindowColors(IsDarkTheme);
        StyleChanged?.Invoke(null, EventArgs.Empty);
    }


    /// <summary>Установить цвет акцента</summary>
    public static void SetAccentColor(Color color)
    {
        _StyleColors.AccentColor = color;
        StyleChanged?.Invoke(null, EventArgs.Empty);
    }



    /// <summary>Найти кисть в ресурсах</summary>
    /// <param name="BrushName">Имя кисти</param>
    public static SolidColorBrush GetBrushFromResource(StyleColors.StyleBrushes BrushName)
    {
        var br = (SolidColorBrush)Application.Current.Resources[BrushName.ToString()];
        return br;
    }


    #region Theme

    /// <summary>
    /// Установить тёмную тему.
    /// </summary>
    public static void SetDarkColorTheme()
    {
        _StyleColors.BackgroundColor = DarkColor;
        _StyleColors.SecondaryBackgroundColor = Lighten(DarkColor, 10);
        _StyleColors.TextColor = WhiteColor;
        _StyleColors.ShadowColor = Colors.Black;
        _StyleColors.DividerColor = Colors.DimGray;

        SetWindowColors(true);
        StyleChanged?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>
    /// Установить светлую тему.
    /// </summary>
    public static void SetLightColorTheme()
    {
        _StyleColors.BackgroundColor = WhiteColor;
        _StyleColors.SecondaryBackgroundColor = WhiteColor;
        _StyleColors.TextColor = DarkColor;
        _StyleColors.ShadowColor = Colors.DimGray;
        _StyleColors.DividerColor = (Color)ColorConverter.ConvertFromString("#FFE0E0E0")!;

        SetWindowColors(false);
        StyleChanged?.Invoke(null, EventArgs.Empty);
    }

    #endregion


    #region Private

    private static void SetWindowColors(bool isDarkTheme)
    {
        var windowBackgroundColor = isDarkTheme
        ? _StyleColors.DarkWindowBackgroundColor
        : _StyleColors.LightWindowBackgroundColor;

        _StyleColors.WindowBackgroundColor = windowBackgroundColor;
        _StyleColors.InactiveWindowBackgroundColor = Lighten(windowBackgroundColor, isDarkTheme ? 5 : 2);

        var foregroungBrush = _BrushLightOrDarkConverter.Convert(new(windowBackgroundColor));
        _StyleColors.WindowForegroundColor = foregroungBrush.Color;
    }


    /// <summary>Взять цвет светлее</summary>
    private static Color Lighten(Color basic, double koef)
    {
        var lighten = Color.FromArgb(255, (byte)(basic.R + (255 - basic.R) / koef),
            (byte)(basic.G + (255 - basic.G) / koef),
            (byte)(basic.B + (255 - basic.B) / koef));
        return lighten;
    }

    /// <summary>Взять цвет темнее</summary>
    private static Color Darken(Color basic, double koef)
    {
        var darken = Color.FromArgb(255, (byte)(basic.R / koef),
            (byte)(basic.G / koef),
            (byte)(basic.B / koef));
        return darken;
    }

    #endregion
}