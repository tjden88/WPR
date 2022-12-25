using System.Drawing;
using WPR.ColorTheme;
using WPR.Interfaces.Themes;
using WPR.Models.Themes;
using MColor = System.Windows.Media.Color;

namespace WPR.UiServices.Themes;

/// <summary>
/// Реализация сервиса для WPF
/// </summary>
public class WPRColorThemeManager : IColorThemeManager
{
    public ColorThemeTypes CurrentColorTheme => StyleHelper.IsDarkTheme ? ColorThemeTypes.Dark : ColorThemeTypes.Light;


    public void SetColorTheme(ColorThemeTypes themeType)
    {
        if (themeType == ColorThemeTypes.Dark)
            StyleHelper.SetDarkColorTheme();
        else
            StyleHelper.SetLightColorTheme();
    }

    public void SetPrimaryColor(Color PrimaryColor) => StyleHelper.SetPrimaryColor(GetMColor(PrimaryColor));

    public void SetAccentColor(Color AccentColor) => StyleHelper.SetAccentColor(GetMColor(AccentColor));

    public Models.Themes.ColorTheme GetCurrentTheme() =>
        new()
        {
            PrimaryColor = GetDColor(StyleHelper.StyleColors.PrimaryColor),
            AccentColor = GetDColor(StyleHelper.StyleColors.AccentColor),
            ThemeType = CurrentColorTheme
        };


    public void SetColorTheme(Models.Themes.ColorTheme theme)
    {
        SetPrimaryColor(theme.PrimaryColor);
        SetAccentColor(theme.AccentColor);
        SetColorTheme(theme.ThemeType);
    }

    /// <summary> Преобразование цвета </summary>
    private static MColor GetMColor(Color color) => MColor.FromArgb(color.A, color.R, color.G, color.B);
    private static Color GetDColor(MColor color) => Color.FromArgb(color.A, color.R, color.G, color.B);
}