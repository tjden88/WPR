using System.Windows.Media;
using WPR.ColorTheme;
using WPR.Domain.Themes;
using WPR.Interfaces.Themes;

namespace WPR.UiServices.Themes;

/// <summary>
/// Реализация сервиса для WPF
/// </summary>
public class WPRColorThemeManager : IColorThemeManager
{
    public ColorThemeTypes CurrentColorTheme => StyleHelper.IsDarkTheme ? ColorThemeTypes.Dark : ColorThemeTypes.Light;


    public void SetThemeType(ColorThemeTypes themeType)
    {
        if (themeType == ColorThemeTypes.Dark)
            StyleHelper.SetDarkColorTheme();
        else
            StyleHelper.SetLightColorTheme();
    }

    public void SetPrimaryColor(string PrimaryColor) => 
        StyleHelper.SetPrimaryColor((Color)ColorConverter.ConvertFromString(PrimaryColor));

    public void SetAccentColor(string AccentColor) => 
        StyleHelper.SetAccentColor((Color)ColorConverter.ConvertFromString(AccentColor));


    public Domain.Themes.ColorTheme GetCurrentTheme() =>
        new()
        {
            PrimaryColor = StyleHelper.StyleColors.PrimaryColor.ToString(),
            AccentColor = StyleHelper.StyleColors.AccentColor.ToString(),
            ThemeType = CurrentColorTheme
        };


    public void SetColorTheme(Domain.Themes.ColorTheme theme)
    {
        SetPrimaryColor(theme.PrimaryColor);
        SetAccentColor(theme.AccentColor);
        SetThemeType(theme.ThemeType);
    }
}