using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPR.MVVM.Converters;

namespace WPR.Converters
{
    /// <summary>
    /// Преобразовывает bool в Visible, если value=true, иначе Collapsed
    /// Если Parameter = !, то всё наоборот делает
    /// </summary>
    [ValueConversion(typeof(bool?), typeof(Visibility))]
    public class BoolToVisibilityConverter : ConverterBase
    {
        protected override object Convert(object v, Type t, object p, CultureInfo c)
        {
            if ((p as string) == "!")
                return (bool) v ? Visibility.Collapsed : Visibility.Visible;

            return (bool)v ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override object ConvertBack(object v, Type t, object p, CultureInfo c)
        {
            if ((p as string) == "!")
                return (Visibility) v != Visibility.Visible;

            return (Visibility)v == Visibility.Visible;
        }
    }
}