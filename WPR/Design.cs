using System;
using System.Windows;
using System.Windows.Media;
using WPR.Services;

namespace WPR
{
    public static class Design
    {
        public enum StyleBrush
        {
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
            AnimationEnterColorBrush
        }

        /// <summary>Установлена ли тёмная тема</summary>
        public static bool IsDarkThemeCurrent => GetBrushFromResource(StyleBrush.BackgroundColorBrush).Color == ColorService.DarkColor;

        /// <summary> Происходит при любом изменении цветовой схемы</summary>
        public static event EventHandler StyleChanged;

        /// <summary>Задать новый рандомный стиль (цветовую палитру) элементам управления</summary>
        public static void SetNewRandomStyle()
        {
            Random rnd = new();
            Color rndColor = Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            SetPrimaryColor(rndColor);
            SetAccentColor(Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
        }

        /// <summary>Установить главный цвет (включая тёмный и светлый)</summary>
        public static void SetPrimaryColor(Color color)
        {
            ColorService.SetNewBrush("PrimaryColorBrush", color);
            ColorService.SetNewBrush("DarkPrimaryColorBrush", ColorService.Darken(color, 1.2));
            ColorService.SetNewBrush("LightPrimaryColorBrush", ColorService.Lighten(color, 1.5));
            ColorService.SetNewBrush("InactiveWindowColorBrush", ColorService.Lighten(color, 2));
            StyleChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>Установить цвет акцента</summary>
        public static void SetAccentColor(Color color)
        {
            ColorService.SetNewBrush("AccentColorBrush", color);
            StyleChanged?.Invoke(null, EventArgs.Empty);
        }



        // TODO: доработать тёмную тему
        /// <summary>
        /// Установить тёмную тему. НЕ ДОРАБОТАНО
        /// </summary>
        public static void SetDarkColorTheme()
        {
            ColorService.SetNewBrush("BackgroundColorBrush", ColorService.DarkColor);
            ColorService.SetNewBrush("PrimaryTextColorBrush", ColorService.WhiteColor);
            //ColorService.SetNewBrush("TextColorBrush", ColorService.DarkColor);
            StyleChanged?.Invoke(null, EventArgs.Empty);
        }

        // TODO: доработать светлую тему
        /// <summary>
        /// Установить светлую тему. НЕ ДОРАБОТАНО
        /// </summary>
        public static void SetLightColorTheme()
        {
            ColorService.SetNewBrush("BackgroundColorBrush", ColorService.WhiteColor);
            ColorService.SetNewBrush("PrimaryTextColorBrush", ColorService.DarkColor);
            //ColorService.SetNewBrush("TextColorBrush", ColorService.WhiteColor);
            StyleChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>Найти кисть в ресурсах</summary>
        /// <param name="Name">Имя кисти</param>
        public static SolidColorBrush GetBrushFromResource(StyleBrush Name)
        {
            return Application.Current.Resources[Name.ToString()] as SolidColorBrush;
        }
    }
}
