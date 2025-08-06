using System.Globalization;
using System.Windows.Data;

namespace WPR.Converters.Base;

/// <summary> Типизированный конвертер </summary>
public class TypeConverter<T>(Converter converter) : IValueConverter
    where T : class
{
    public Converter Converter { get; init; } = converter;

    public T Convert(T value, object p = null) => Converter.Convert(value, typeof(T), p, CultureInfo.CurrentCulture) as T;

    object IValueConverter.Convert(object v, Type t, object p, CultureInfo c) => Converter.Convert(v, t, p, c);

    object IValueConverter.ConvertBack(object v, Type t, object p, CultureInfo c) => Converter.ConvertBack(v, t, p, c);
}