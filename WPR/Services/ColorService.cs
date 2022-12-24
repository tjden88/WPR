﻿using System.Windows;
using System.Windows.Media;

namespace WPR.Services;

internal static class ColorService
{

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


    ///// <summary> Установить новую кисть темы </summary>
    //internal static void SetNewBrush(Design.StyleBrushes Brushes, Color color)
    //{
    //    var brushName = Brushes.ToString();
    //    var b = new SolidColorBrush(color);
    //    b.Freeze();
    //    Application.Current.Resources[brushName] = b;
    //    Application.Current.Resources[brushName.Replace("Brushes","")] = color;
    //}
}