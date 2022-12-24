using System.Globalization;
using System.Windows.Data;

namespace WPR.MVVM.Converters.Base;

/// <summary> Типизированный конвертер </summary>
public class TypeConverter<T> : IValueConverter where T : class
{
    private readonly Converter _Converter;

    public TypeConverter(Converter Converter)
    {
        _Converter = Converter;
    }

    public T Convert(T value, Type t, object p, CultureInfo c) => _Converter.Convert(value, t, p, c) as T;

    object IValueConverter.Convert(object v, Type t, object p, CultureInfo c) => _Converter.Convert(v, t, p, c);

    object IValueConverter.ConvertBack(object v, Type t, object p, CultureInfo c) => _Converter.ConvertBack(v, t, p, c);
}