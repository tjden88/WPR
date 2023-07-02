using System.Globalization;
using System.Windows.Data;
using WPR.Converters.Base;

namespace WPR.Converters;

/// <summary>
/// Истина, если значение - не пустая ссылка
/// </summary>

[ValueConversion(typeof(object), typeof(bool))]
public class ValueIsNotNullConverter : Converter
{
    public override object Convert(object v, Type t, object p, CultureInfo c) => v != null;
}