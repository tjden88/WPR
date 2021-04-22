using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using WPR.MVVM;
using WPR.Services;

namespace WPR.Converters
{
    /// <summary>
    /// Возвращает светлую или тёмную кисть в зависимости от значения
    /// Пример: вернуть светлый цвет текста если фон тёмный
    /// </summary>
    [ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
    public class BrushLightOrDarkConverter : ConverterBase
    {
        public SolidColorBrush HighValue { get; set; } = new(ColorService.WhiteColor);

        public SolidColorBrush LowValue { get; set; } = new(ColorService.DarkColor);

        protected override object Convert(object v, Type t, object p, CultureInfo c)
        {
            if (v is not SolidColorBrush solidColorBrush) return null;

            var color = solidColorBrush.Color;

            var brightness = 0.3 * color.R + 0.59 * color.G + 0.11 * color.B;

            return brightness > 123 ? LowValue : HighValue;
        }

        protected override object ConvertBack(object v, Type t, object p, CultureInfo c) => Convert(v, t, p, c);
    }
}