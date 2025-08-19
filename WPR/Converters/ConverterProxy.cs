using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPR.Converters;

/// <summary>
/// Использовать в MultiBinding для вызова IValueConverter из ViewModel.
/// </summary>
public sealed class ConverterProxy : IMultiValueConverter
{
    // values[0] — основное значение
    // values[1] — IValueConverter (из VM)
    // values[2] — необязательный параметр конвертера (если нужен)
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var value = values.Length > 0 ? values[0] : Binding.DoNothing;
        var converter = values.Length > 1 ? values[1] as IValueConverter : null;
        var param = values.Length > 2 && values[2] != DependencyProperty.UnsetValue ? values[2] : parameter;

        return converter == null 
            ? Binding.DoNothing 
            : converter.Convert(value, targetType, param, culture);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}