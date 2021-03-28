using System;
using System.Globalization;
using System.Windows.Data;

namespace WPR.Styles.Converters
{
    /// <summary>
    /// Возвращает обратное значение
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolNotConverter : ValueConverter
    {
        protected  override object Convert(object v, Type t, object p, CultureInfo c) => !(bool)v;

        protected override object ConvertBack(object v, Type t, object p, CultureInfo c) => !(bool)v;
    }
}