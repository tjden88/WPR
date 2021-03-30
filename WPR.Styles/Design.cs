using System;
using System.Windows;
using System.Windows.Media;

namespace WPR.Styles
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
            AnimationEnterColorBrush
        }

        /// <summary>
        /// Происходит при любом изменении цветовой схемы
        /// </summary>
        public static event EventHandler StyleChanged;

        /// <summary>
        /// Задать новый рандомный стиль (цветовую палитру) элементам управления
        /// </summary>
        public static void SetNewRandomStyle()
        {
            Random rnd = new();

            Color rndColor = Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));

            SetPrimaryColor(rndColor);
            SetAccentColor(Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
        }

        //public static void SetPrimaryColor(Color color)
        //{
        //    Application.Current.Resources["PrimaryColorBrush"] = new SolidColorBrush(color);
        //    Application.Current.Resources["DarkPrimaryColorBrush"] = new SolidColorBrush(Darken(color, 1.2));
        //    Application.Current.Resources["LightPrimaryColorBrush"] = new SolidColorBrush(Lighten(color, 1.5));
        //    StyleChanged?.Invoke(null, EventArgs.Empty);
        //}
        public static void SetPrimaryColor(Color color)
        {
            Application.Current.Resources["PrimaryColor"] = color;
            Application.Current.Resources["DarkPrimaryColorBrush"] = new SolidColorBrush(Darken(color, 1.2));
            Application.Current.Resources["LightPrimaryColorBrush"] = new SolidColorBrush(Lighten(color, 1.5));
            StyleChanged?.Invoke(null, EventArgs.Empty);
        }
        public static void SetAccentColor(Color color)
        {
            Application.Current.Resources["AccentColorBrush"] = new SolidColorBrush(color);
            StyleChanged?.Invoke(null, EventArgs.Empty);
        }

        // Взять цвет светлее
        private static Color Lighten(Color basic, double koef)
        {
            Color lighten = Color.FromArgb(255, (byte)(basic.R + ((255 - basic.R) / koef)),
                (byte)(basic.G + ((255 - basic.G) / koef)),
                (byte)(basic.B + ((255 - basic.B) / koef)));
           return lighten;
        }

        // Взять цвет темнее
        private static Color Darken(Color basic, double koef)
        {
            Color darken = Color.FromArgb(255, (byte)(basic.R / koef),
                 (byte)(basic.G / koef),
                 (byte)(basic.B / koef));
            return darken;
        }

        // TODO: доработать тёмную тему
        /// <summary>
        /// Установить тёмную тему. НЕ ДОРАБОТАНО
        /// </summary>
        public static void SetDarkColorTheme()
        {
            Application.Current.Resources["BackgroundColorBrush"] = new SolidColorBrush(Colors.Black);
            Application.Current.Resources["PrimaryTextColorBrush"] = new SolidColorBrush(Colors.White);
            Application.Current.Resources["TextColorBrush"] = new SolidColorBrush(Colors.Black);
            StyleChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Найти кисть в ресурсах
        /// </summary>
        /// <param name="Name">Имя кисти</param>
        public static SolidColorBrush GetBrushFromResource(StyleBrush Name)
        {
            return (Application.Current.Resources[Name.ToString()] as SolidColorBrush) ?? null;
        }
    }
}
