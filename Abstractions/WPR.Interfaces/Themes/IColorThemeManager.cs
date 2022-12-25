using System.Drawing;

namespace WPR.Interfaces.Themes;

/// <summary>
/// Управление цветовыми схемами приложения
/// </summary>
public interface IColorThemeManager
{
    /// <summary> Текущая цветовая тема </summary>
    ColorThemeTypes CurrentColorTheme { get; }


    /// <summary> Установить тёмную или светлую тему </summary>
    void SetColorTheme(ColorThemeTypes themeType);


    /// <summary> Установить основной цвет </summary>
    void SetPrimaryColor(Color PrimaryColor);

}