using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPR.MVVM.Converters.Base;

namespace WPR.MVVM.Converters;

/// <summary>
/// Скрывает объект, если Value - пустая ссылка или пустая строка
/// Если параметр = "!" - наоборот делает
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility))]
public class ValueIsNullToVisibilityConverter : Converter
{
    public override object Convert(object v, Type t, object p, CultureInfo c)
    {
        var isNull = v is null || Equals(v, string.Empty);
        var visible = (p is "!" && isNull) || (p is not "!" && !isNull);

        return visible? Visibility.Visible : Visibility.Collapsed;
    }
}