using System;
using System.Windows;
using System.Windows.Media;
using WPR.Services;

namespace WPR;

public static class Design
{
    private static readonly Color _DarkColor = GetBrushFromResource(StyleBrush.DarkBrush).Color; // Кисть тёмной темы
    private static readonly Color _WhiteColor = GetBrushFromResource(StyleBrush.WhiteBrush).Color; // Кисть светлой темы

    public enum StyleBrush
    {
        None,
        PrimaryColorBrush,
        DarkPrimaryColorBrush,
        LightPrimaryColorBrush,
        AccentColorBrush,
        TextColorBrush,
        PrimaryTextColorBrush,
        SecondaryTextColorBrush,
        DividerColorBrush,
        MenuBodyBrush,
        BackgroundColorBrush,
        InactiveWindowColorBrush,
        AnimationEnterColorBrush,
        WhiteBrush,
        DarkBrush
    }

    /// <summary>Установлена ли тёмная тема</summary>
    public static bool IsDarkThemeCurrent => GetBrushFromResource(StyleBrush.BackgroundColorBrush).Color == _DarkColor;


    /// <summary> Происходит при любом изменении цветовой схемы</summary>
    public static event EventHandler StyleChanged;


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
        ColorService.SetNewBrush(StyleBrush.PrimaryColorBrush, color);

        var darken = ColorService.Darken(color, 1.2);
        ColorService.SetNewBrush(StyleBrush.DarkPrimaryColorBrush, darken);

        var lighten = ColorService.Lighten(color, 1.5);
        ColorService.SetNewBrush(StyleBrush.LightPrimaryColorBrush, lighten);

        var inactiveWindowColor = ColorService.Lighten(color, 1.3);
        ColorService.SetNewBrush(StyleBrush.InactiveWindowColorBrush, inactiveWindowColor);

        StyleChanged?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>Установить цвет акцента</summary>
    public static void SetAccentColor(Color color)
    {
        ColorService.SetNewBrush(StyleBrush.AccentColorBrush, color);
        StyleChanged?.Invoke(null, EventArgs.Empty);
    }



    // TODO: доработать тёмную тему
    /// <summary>
    /// Установить тёмную тему. НЕ ДОРАБОТАНО
    /// </summary>
    [Obsolete("Не доработано")]
    public static void SetDarkColorTheme()
    {
        ColorService.SetNewBrush(StyleBrush.BackgroundColorBrush, _DarkColor);
        ColorService.SetNewBrush(StyleBrush.PrimaryTextColorBrush, _WhiteColor);
        StyleChanged?.Invoke(null, EventArgs.Empty);
    }

    // TODO: доработать светлую тему
    /// <summary>
    /// Установить светлую тему. НЕ ДОРАБОТАНО
    /// </summary>
    [Obsolete("Не доработано")]
    public static void SetLightColorTheme()
    {
        ColorService.SetNewBrush(StyleBrush.BackgroundColorBrush, _WhiteColor);
        ColorService.SetNewBrush(StyleBrush.PrimaryTextColorBrush, _DarkColor);
        StyleChanged?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>Найти кисть в ресурсах</summary>
    /// <param name="Name">Имя кисти</param>
    public static SolidColorBrush GetBrushFromResource(StyleBrush Name)
    {
        var br = (SolidColorBrush) Application.Current.Resources[Name.ToString()];
        br?.Freeze();
        return br;
    }
}