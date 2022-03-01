using System.Globalization;

namespace WPR.MVVM.Converters
{
    /// <summary>Базовая реализация конвертера</summary>
    public class ValueConverter : ConverterBase
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
}