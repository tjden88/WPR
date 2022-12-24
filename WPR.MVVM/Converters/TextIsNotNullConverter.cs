using System;
using System.Globalization;
using System.Windows.Data;
using WPR.MVVM.Converters.Base;

namespace WPR.Converters
{
    /// <summary>
    /// Истина, если текст не пуст и не состоит из одних пробелов
    /// </summary>
    [ValueConversion(typeof(string), typeof(bool))]
    public class TextIsNotNullConverter : Converter
    {
        protected override object Convert(object v, Type t, object p, CultureInfo c) =>
            v != null && !string.IsNullOrWhiteSpace(v.ToString());
    }
}
