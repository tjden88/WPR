using System;
using System.Windows;
using System.Windows.Media;
using WPR.Domain.Models.Themes;

namespace WPR.ColorTheme;

/// <summary>
/// Установка и изменение цветовой темы
/// </summary>
public static class StyleHelper
{
    /// <summary> Цвета текущей сессии </summary>
    public static readonly StyleColors StyleColors = (StyleColors)Application.Current.Resources["StyleColors"];

    /// <summary> Происходит при любом изменении цветовой схемы</summary>
    public static event EventHandler StyleChanged;

    /// <summary>Установлена ли тёмная тема</summary>
    public static bool IsDarkTheme => StyleColors._DarkColor == StyleColors.BackgroundColor;


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
        StyleColors.PrimaryColor = color;
        StyleColors.DarkPrimaryColor = Darken(color, 1.2);
        StyleColors.LightPrimaryColor = Lighten(color, 1.5);

        SetWindowColors(IsDarkTheme);
        StyleChanged?.Invoke(null, EventArgs.Empty);
    }


    /// <summary>Установить цвет акцента</summary>
    public static void SetAccentColor(Color color)
    {
        StyleColors.AccentColor = color;
        StyleChanged?.Invoke(null, EventArgs.Empty);
    }



    /// <summary>Найти кисть в ресурсах</summary>
    /// <param name="BrushName">Имя кисти</param>
    public static SolidColorBrush GetBrushFromResource(StyleBrushes BrushName)
    {
        var br = Application.Current.Resources[BrushName.ToString()] as SolidColorBrush;
        return br;
    }


    #region Theme

    /// <summary>
    /// Установить тёмную тему.
    /// </summary>
    public static void SetDarkColorTheme()
    {
        StyleColors.BackgroundColor = StyleColors._DarkBackgroundColor;
        StyleColors.SecondaryBackgroundColor = StyleColors._DarkSecondaryBackgroundColor;
        StyleColors.TextColor = StyleColors._DarkTextColor;
        StyleColors.ShadowColor = StyleColors._DarkShadowColor;
        StyleColors.DividerColor = StyleColors._DarkDividerColor;

        SetWindowColors(true);
        StyleChanged?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>
    /// Установить светлую тему.
    /// </summary>
    public static void SetLightColorTheme()
    {
        StyleColors.BackgroundColor = StyleColors._LightBackgroundColor;
        StyleColors.SecondaryBackgroundColor = StyleColors._LightSecondaryBackgroundColor;
        StyleColors.TextColor = StyleColors._LightTextColor;
        StyleColors.ShadowColor = StyleColors._LightShadowColor;
        StyleColors.DividerColor = StyleColors._LightDividerColor;

        SetWindowColors(false);
        StyleChanged?.Invoke(null, EventArgs.Empty);
    }

    #endregion


    #region Private

    private static void SetWindowColors(bool isDarkTheme)
    {
        var windowBackgroundColor = isDarkTheme
        ? StyleColors._DarkWindowTitleBackgroundColor
        : StyleColors.PrimaryColor;

        StyleColors.WindowTitleBackgroundColor = windowBackgroundColor;
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