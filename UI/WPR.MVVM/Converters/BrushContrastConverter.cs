using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using WPR.MVVM.Converters.Base;

namespace WPR.MVVM.Converters;

/// <summary>
/// Возвращает светлую или тёмную кисть в зависимости от значения
/// Пример: вернуть светлый цвет текста если фон тёмный
/// </summary>
[ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
public class BrushContrastConverter : Converter
{
    private static readonly ColorContrastConverter _ColorContrastConverter = new();

    public SolidColorBrush HighValue { get; set; } = new(Colors.White);
    public SolidColorBrush LowValue { get; set; } = new(Colors.Black);

    public override object Convert(object v, Type t, object p, CultureInfo c)
    {
        if (v is not SolidColorBrush solidColorBrush) return null;
        return _ColorContrastConverter.IsContrastLow(solidColorBrush.Color)
            ? LowValue
            : HighValue;
    }

    public override object ConvertBack(object v, Type t, object p, CultureInfo c) => Convert(v, t, p, c);
}