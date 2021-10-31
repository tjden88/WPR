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
            AnimationEnterColorBrush
        }

        /// <summary>Установлена ли тёмная тема</summary>
        public static bool IsDarkThemeCurrent => GetBrushFromResource(StyleBrush.BackgroundColorBrush).Color == ColorService.DarkColor;

        /// <summary> Происходит перед изменении кисти</summary>
        public static Action<StyleBrush, Color> BeforeBrushChanged;
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
            BeforeBrushChanged?.Invoke(StyleBrush.PrimaryColorBrush, color);
            ColorService.SetNewBrush(StyleBrush.PrimaryColorBrush, color);

            var darken = ColorService.Darken(color, 1.2);
            BeforeBrushChanged?.Invoke(StyleBrush.DarkPrimaryColorBrush, darken);
            ColorService.SetNewBrush(StyleBrush.DarkPrimaryColorBrush, darken);

            var lighten = ColorService.Lighten(color, 1.5);
            BeforeBrushChanged?.Invoke(StyleBrush.LightPrimaryColorBrush, lighten);
            ColorService.SetNewBrush(StyleBrush.LightPrimaryColorBrush, lighten);

            var inactiveWindowColor = ColorService.Lighten(color, 1.5);
            BeforeBrushChanged?.Invoke(StyleBrush.InactiveWindowColorBrush, inactiveWindowColor);
            ColorService.SetNewBrush(StyleBrush.InactiveWindowColorBrush, inactiveWindowColor);

            StyleChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>Установить цвет акцента</summary>
        public static void SetAccentColor(Color color)
        {
            BeforeBrushChanged?.Invoke(StyleBrush.AccentColorBrush, color);
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
            BeforeBrushChanged?.Invoke(StyleBrush.BackgroundColorBrush, ColorService.DarkColor);
            ColorService.SetNewBrush(StyleBrush.BackgroundColorBrush, ColorService.DarkColor);

            BeforeBrushChanged?.Invoke(StyleBrush.PrimaryTextColorBrush, ColorService.WhiteColor);
            ColorService.SetNewBrush(StyleBrush.PrimaryTextColorBrush, ColorService.WhiteColor);
            //ColorService.SetNewBrush("TextColorBrush", ColorService.DarkColor);
            StyleChanged?.Invoke(null, EventArgs.Empty);
        }

        // TODO: доработать светлую тему
        /// <summary>
        /// Установить светлую тему. НЕ ДОРАБОТАНО
        /// </summary>
        [Obsolete("Не доработано")]
        public static void SetLightColorTheme()
        {
            BeforeBrushChanged?.Invoke(StyleBrush.BackgroundColorBrush, ColorService.WhiteColor);
            ColorService.SetNewBrush(StyleBrush.BackgroundColorBrush, ColorService.WhiteColor);

            BeforeBrushChanged?.Invoke(StyleBrush.PrimaryTextColorBrush, ColorService.DarkColor);
            ColorService.SetNewBrush(StyleBrush.PrimaryTextColorBrush, ColorService.DarkColor);
            //ColorService.SetNewBrush("TextColorBrush", ColorService.WhiteColor);
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
}
