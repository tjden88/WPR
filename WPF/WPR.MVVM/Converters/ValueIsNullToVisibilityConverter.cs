using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPR.MVVM.Converters.Base;

namespace WPR.MVVM.Converters;

/// <summary>
/// Скрывает объект, если Value - пустая ссылка
/// Если параметр = "!" - наоборот делает
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility))]
public class ValueIsNullToVisibilityConverter : Converter
{
    public override object Convert(object v, Type t, object p, CultureInfo c)
    {
        var visible = (p is "!" && v is null) || (p is not "!" && v is not null);

        return visible? Visibility.Visible : Visibility.Collapsed;
    }
}