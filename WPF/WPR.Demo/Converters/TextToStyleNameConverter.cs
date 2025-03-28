using System;
using System.Globalization;
using WPR.MVVM.Converters.Base;

namespace WPR.Demo.Converters
{
    internal class TextToStyleNameConverter : Converter
    {
        public override object Convert(object v, Type t, object p, CultureInfo c)
        {
            if (v is not string {Length: > 0} name)
                return "<Нет>";

            return Convert(name);
        }

        public string Convert(string value) => $"Style=\"{{StaticResource {value}\"}}";
    }
}
