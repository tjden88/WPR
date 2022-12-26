using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using WPR.MVVM.Converters.Base;

namespace WPR.MVVM.Converters;

/// <summary>
/// Возвращает светлый или тёмный в зависимости от значения
/// Пример: вернуть светлый цвет текста если фон тёмный
/// </summary>
[ValueConversion(typeof(Color), typeof(Color))]
public class ColorContrastConverter : Converter
{
    public Color HighValue { get; set; } = Colors.White;
    public Color LowValue { get; set; } = Colors.Black;

    internal bool IsContrastLow(Color color)
    {
        var brightness = 0.3 * color.R + 0.59 * color.G + 0.11 * color.B;
        return brightness > 123;
    }

    public override object Convert(object v, Type t, object p, CultureInfo c)
    {
        if (v is not Color color) return default(Color);
        return IsContrastLow(color) ? LowValue : HighValue;
    }

    public override object ConvertBack(object v, Type t, object p, CultureInfo c) => Convert(v, t, p, c);
}