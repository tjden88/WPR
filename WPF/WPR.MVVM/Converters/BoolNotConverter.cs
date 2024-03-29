﻿using System.Globalization;
using System.Windows.Data;
using WPR.MVVM.Converters.Base;

namespace WPR.MVVM.Converters;

/// <summary>
/// Возвращает обратное значение
/// </summary>
[ValueConversion(typeof(bool), typeof(bool))]
public class BoolNotConverter : Converter
{
    public override object Convert(object v, Type t, object p, CultureInfo c) => !(bool)v;

    public override object ConvertBack(object v, Type t, object p, CultureInfo c) => !(bool)v;
}