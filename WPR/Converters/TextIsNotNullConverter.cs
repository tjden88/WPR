using System;
using System.Globalization;
using System.Windows.Data;
using WPR.MVVM;

namespace WPR.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class TextIsNotNullConverter : ConverterBase
    {

        protected override object Convert(object v, Type t, object p, CultureInfo c)
        {
            return v != null && !string.IsNullOrEmpty(v.ToString());
        }
    }
}
