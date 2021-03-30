﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WPR.Styles.Services
{
    internal static class ColorService
    {
        internal static Color DarkColor => (Color)ColorConverter.ConvertFromString("#FF383838");
        internal static Color WhiteColor => (Color)ColorConverter.ConvertFromString("#FFFFFF");

        /// <summary>Взять цвет светлее</summary>
        internal static Color Lighten(Color basic, double koef)
        {
            Color lighten = Color.FromArgb(255, (byte)(basic.R + ((255 - basic.R) / koef)),
                (byte)(basic.G + ((255 - basic.G) / koef)),
                (byte)(basic.B + ((255 - basic.B) / koef)));
            return lighten;
        }

        /// <summary>Взять цвет темнее</summary>
        internal static Color Darken(Color basic, double koef)
        {
            Color darken = Color.FromArgb(255, (byte)(basic.R / koef),
                (byte)(basic.G / koef),
                (byte)(basic.B / koef));
            return darken;
        }

        internal static void SetNewBrush(string BrushName, Color color)
        {
            var b = new SolidColorBrush(color);
            b.Freeze();
            Application.Current.Resources[BrushName] = b;
            Application.Current.Resources[BrushName.Replace("Brush","")] = color;
        }
    }
}