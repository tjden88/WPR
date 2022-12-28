using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPR.MVVM.Converters.Base;

namespace WPR.MVVM.Converters;

/// <summary>
/// Скрывает объект, если Value - пустая ссылка
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility))]
public class ValueIsNullToVisibilityConverter : Converter
{
    public override object Convert(object v, Type t, object p, CultureInfo c) =>
        v is null ? Visibility.Collapsed : Visibility.Visible;
}