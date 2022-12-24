using System.Globalization;
using System.Windows.Data;
using WPR.MVVM.Converters.Base;

namespace WPR.MVVM.Converters
{
    /// <summary>Превращает число в отрицательное</summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class SignPlusMinusConverter : Converter
    {
        protected override object Convert(object v, Type t, object p, CultureInfo c) => -(double)v;

        protected override object ConvertBack(object v, Type t, object p, CultureInfo c) => -(double)v;
    }
}
