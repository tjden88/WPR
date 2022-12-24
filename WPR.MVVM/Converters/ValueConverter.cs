using System.Globalization;
using WPR.MVVM.Converters.Base;

namespace WPR.MVVM.Converters;

/// <summary>Конвертации значения лямбда - функцией</summary>
public class ValueConverter : Converter
{
    private readonly Func<object, Type, object, CultureInfo, object> _ConvertFunction;
    private readonly Func<object, Type, object, CultureInfo, object> _ConvertBackFunction;

    public ValueConverter(Func<object, Type, object, CultureInfo, object> ConvertFunction, Func< object, Type, object, CultureInfo, object> ConvertBackFunction = null)
    {
        _ConvertFunction = ConvertFunction;
        _ConvertBackFunction = ConvertBackFunction;
    }

    protected override object Convert(object v, Type t, object p, CultureInfo c)
    {
        return _ConvertFunction?.Invoke(v, t, p, c);
    }

    protected override object ConvertBack(object v, Type t, object p, CultureInfo c)
    {
        return _ConvertFunction?.Invoke(v, t, p, c) ?? base.ConvertBack(v, t, p, c);
    }
}