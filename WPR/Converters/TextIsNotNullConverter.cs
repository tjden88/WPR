using System;
using System.Globalization;
using System.Windows.Data;
using WPR.MVVM.Converters;

namespace WPR.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class TextIsNotNullConverter : ConverterBase
    {
        protected override object Convert(object v, Type t, object p, CultureInfo c) =>
            v != null && !string.IsNullOrEmpty(v.ToString());
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class ParameterIsNotNullConverter : ConverterBase
    {
        protected override object Convert(object v, Type t, object p, CultureInfo c) => p != null;

        protected override object ConvertBack(object v, Type t, object p, CultureInfo c) => v is true ? null : p;
    }
}
