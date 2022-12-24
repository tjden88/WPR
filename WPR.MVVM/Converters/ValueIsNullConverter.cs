using System.Globalization;
using System.Windows.Data;
using WPR.MVVM.Converters.Base;

namespace WPR.MVVM.Converters;

/// <summary>
/// Истина, если значение - пустая ссылка
/// </summary>

[ValueConversion(typeof(object), typeof(bool))]
public class ValueIsNullConverter : Converter
{
    protected override object Convert(object v, Type t, object p, CultureInfo c) => v == null;
}