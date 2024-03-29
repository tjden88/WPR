﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPR.MVVM.Converters.Base;

namespace WPR.MVVM.Converters;

/// <summary>
/// Преобразовывает bool в Visible, если value=true, иначе - значение свойства HiddenVisibility (по умолчанию Collapsed)
/// Если Parameter = !, то всё наоборот делает
/// </summary>
[ValueConversion(typeof(bool?), typeof(Visibility))]
public class BoolToVisibilityConverter : Converter
{
    /// <summary> Результат преобразования (по умолчанию Collapsed) </summary>
    public Visibility HiddenVisibility { get; set; } = Visibility.Collapsed;

    public override object Convert(object v, Type t, object p, CultureInfo c)
    {
        if (p as string == "!")
            return (bool) v ? HiddenVisibility : Visibility.Visible;

        return (bool)v ? Visibility.Visible : HiddenVisibility;
    }

    public override object ConvertBack(object v, Type t, object p, CultureInfo c)
    {
        if (p as string == "!")
            return (Visibility) v != Visibility.Visible;

        return (Visibility)v == Visibility.Visible;
    }
}