﻿using System.Drawing;
using WPR.Models.Themes;

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


    /// <summary> Установить цвет акцента </summary>
    void SetAccentColor(Color AccentColor);


    /// <summary> Получить текущую цветовую схему </summary>
    ColorTheme GetCurrentTheme();


    /// <summary> Установить новую цветовую схему </summary>
    void SetColorTheme(ColorTheme theme);
}