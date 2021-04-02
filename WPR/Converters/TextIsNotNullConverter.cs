using System;
using System.Globalization;
using System.Windows.Data;

namespace WPR.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class TextIsNotNullConverter : ValueConverter
    {

        protected override object Convert(object v, Type t, object p, CultureInfo c)
        {
            return v != null && !string.IsNullOrEmpty(v.ToString());
        }
    }
}
