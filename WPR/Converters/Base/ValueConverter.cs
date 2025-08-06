using System.Globalization;

namespace WPR.Converters.Base;

/// <summary>Конвертации значения лямбда - функцией</summary>
public class ValueConverter(
    Func<object, Type, object, CultureInfo, object> convertFunction,
    Func<object, Type, object, CultureInfo, object> convertBackFunction = null)
    : Converter
{
    public override object Convert(object v, Type t, object p, CultureInfo c)
    {
        return convertFunction?.Invoke(v, t, p, c);
    }


    public override object ConvertBack(object v, Type t, object p, CultureInfo c)
    {
        return convertBackFunction?.Invoke(v, t, p, c) ?? base.ConvertBack(v, t, p, c);
    }
}