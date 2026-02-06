using System.Globalization;
using System.Windows.Data;
using WPR.Converters.Base;

namespace WPR.Converters;

/// <summary>
/// Если текст пуст (string.IsNullOrWhiteSpace()) вернёт значение параметра
/// </summary>
[ValueConversion(typeof(string), typeof(string))]
public class EmptyTextToParameterValueConverter : Converter
{
    public override object Convert(object v, Type t, object p, CultureInfo c) =>
        string.IsNullOrWhiteSpace(v.ToString())
            ? p : v;
}