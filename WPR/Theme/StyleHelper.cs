using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPR.Theme;

/// <summary>
/// Установка и изменение цветовой темы
/// </summary>
public static class StyleHelper
{
    private static ColorScheme _Scheme = ColorScheme.Light;
    private static bool _IsStyleChangedInvokable = true;

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

    /// <summary>Найти кисть в ресурсах</summary>
    /// <param name="BrushName">Имя кисти</param>
    public static SolidColorBrush GetBrushFromResource(StyleBrushes BrushName)
    {
        var br = Application.Current.Resources[BrushName.ToString()] as SolidColorBrush;
        return br;
    }


    #region Fonts


    /// <summary>
    /// Установить базовый шрифт для всех элементов управления.
    /// </summary>
    public static void SetBaseFont(Fonts font)
    {
        var fontFamily = Application.Current.Resources[font.ToString()] as FontFamily;

        if (Application.Current.Resources["BaseControl"] is not Style style || fontFamily is null) 
            throw new InvalidOperationException($"Стиль или шрифт {font} не найдены в ресурсах приложения.");

        var existing = style.Setters
            .OfType<Setter>()
            .FirstOrDefault(s => s.Property == Control.FontFamilyProperty);

        if (existing != null)
            existing.Value = fontFamily;
        else
            style.Setters.Add(new Setter(Control.FontFamilyProperty, fontFamily));
    }

    #endregion

    #region Colors


    /// <summary>Установить главный цвет (включая тёмный и светлый)</summary>
    public static void SetPrimaryColor(Color color)
    {
        StyleColors.PrimaryColor = color;
        StyleColors.DarkPrimaryColor = Darken(color, 1.5);
        StyleColors.LightPrimaryColor = Lighten(color, 1.5);

        SetWindowColors(IsDarkTheme);
        if (_IsStyleChangedInvokable) StyleChanged?.Invoke(null, EventArgs.Empty);
    }


    /// <summary>Установить цвет акцента</summary>
    public static void SetAccentColor(Color color)
    {
        StyleColors.AccentColor = color;
        if (_IsStyleChangedInvokable) StyleChanged?.Invoke(null, EventArgs.Empty);
    }


    #endregion

    #region Theme

    /// <summary> Получить текущую цветовую схему </summary>
    public static ColorTheme GetCurrentTheme() => new(StyleColors.PrimaryColor.ToString(), StyleColors.AccentColor.ToString(), _Scheme);


    /// <summary> Установить новую цветовую схему </summary>
    public static void SetColorTheme(ColorTheme theme)
    {
        _IsStyleChangedInvokable = false; // Отключаем вызов события StyleChanged
        SetPrimaryColor((Color)ColorConverter.ConvertFromString(theme.PrimaryColor)!);
        SetAccentColor((Color)ColorConverter.ConvertFromString(theme.AccentColor)!);
        SetColorScheme(theme.Scheme);
        _IsStyleChangedInvokable = true; // Включаем вызов события StyleChanged
        StyleChanged?.Invoke(null, EventArgs.Empty);
    }

    #endregion

    #region Scheme

    /// <summary> Установить тёмную, светлую или системную тему </summary>
    public static void SetColorScheme(ColorScheme scheme)
    {
        switch (scheme)
        {
            case ColorScheme.Dark:
                SetDarkColorTheme();
                break;
            case ColorScheme.Light:
                SetLightColorTheme();
                break;
            case ColorScheme.Auto:
                SetSystemTheme();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scheme), scheme, null);
        }
        _Scheme = scheme;
    }

    /// <summary>
    /// Установить тёмную тему.
    /// </summary>
    private static void SetDarkColorTheme()
    {
        StyleColors.BackgroundColor = StyleColors._DarkBackgroundColor;
        StyleColors.SecondaryBackgroundColor = StyleColors._DarkSecondaryBackgroundColor;
        StyleColors.TextColor = StyleColors._DarkTextColor;
        StyleColors.ShadowColor = StyleColors._DarkShadowColor;
        StyleColors.DividerColor = StyleColors._DarkDividerColor;

        SetWindowColors(true);
        if (_IsStyleChangedInvokable) StyleChanged?.Invoke(null, EventArgs.Empty);
    }


    /// <summary>
    /// Установить светлую тему.
    /// </summary>
    private static void SetLightColorTheme()
    {
        StyleColors.BackgroundColor = StyleColors._LightBackgroundColor;
        StyleColors.SecondaryBackgroundColor = StyleColors._LightSecondaryBackgroundColor;
        StyleColors.TextColor = StyleColors._LightTextColor;
        StyleColors.ShadowColor = StyleColors._LightShadowColor;
        StyleColors.DividerColor = StyleColors._LightDividerColor;

        SetWindowColors(false);
        if (_IsStyleChangedInvokable) StyleChanged?.Invoke(null, EventArgs.Empty);
    }


    /// <summary>
    /// Установить тему как в системе
    /// </summary>
    private static void SetSystemTheme()
    {
        var systemTheme = IsLightSystemTheme();
        if (systemTheme) 
            SetLightColorTheme();
        else 
            SetDarkColorTheme();
    }

    #endregion

    #region Private

    private static void SetWindowColors(bool isDarkTheme)
    {
        var windowBackgroundColor = isDarkTheme
        ? StyleColors._DarkWindowTitleBackgroundColor
        : StyleColors._LightWindowTitleBackgroundColor;

        StyleColors.WindowTitleBackgroundColor = windowBackgroundColor;

        StyleColors.ShadowColor = isDarkTheme ? StyleColors._DarkShadowColor : StyleColors._LightShadowColor;
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

    /// <summary> Определить системную тему </summary>
    private static bool IsLightSystemTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        var value = key?.GetValue("AppsUseLightTheme");
        return value is > 0;
    }

    #endregion
}