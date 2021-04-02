using System;
using System.Globalization;
using System.Windows.Data;

namespace WPR.Converters
{
    /// <summary>Превращает число в отрицательное</summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class SignPlusMinusConverter : ValueConverter
    {
        protected override object Convert(object v, Type t, object p, CultureInfo c)
        {
            return -(double)v;
        }

        protected override object ConvertBack(object v, Type t, object p, CultureInfo c)
        {
            return -(double)v;
        }
    }
}
