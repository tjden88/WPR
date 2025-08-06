using System.Globalization;
using System.Windows.Data;
using WPR.Converters.Base;

namespace WPR.Converters;

/// <summary>
/// Истина, если значение - пустая ссылка
/// </summary>

[ValueConversion(typeof(object), typeof(bool))]
public class ValueIsNullConverter : Converter
{
    public override object Convert(object v, Type t, object p, CultureInfo c) => v == null;
}