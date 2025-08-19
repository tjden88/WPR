using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WPR.Converters.Base;

/// <summary>
/// Универсальный шаблонный конвертер для WPF.
/// Позволяет описывать логику преобразования прямо через делегаты.
/// Используется как MarkupExtension, чтобы не плодить кучу классов.
/// </summary>
public class ValueConverter<TSource, TTarget> : MarkupExtension, IValueConverter
{
    private readonly Func<TSource, object, TTarget> _convert;
    private readonly Func<TTarget, object, TSource> _convertBack;

    /// <summary>
    /// Статический параметр конвертера (если нужен).
    /// Если в Binding передан ConverterParameter, он имеет приоритет.
    /// </summary>
    public object StaticParameter { get; init; }

    #region Конструкторы

    /// <summary>
    /// Только прямое преобразование.
    /// </summary>
    public ValueConverter(Func<TSource, TTarget> convert)
    {
        _convert = (v, _) => convert(v);
    }

    /// <summary>
    /// Прямое преобразование с учётом параметра.
    /// </summary>
    public ValueConverter(Func<TSource, object, TTarget> convert)
    {
        _convert = convert;
    }

    /// <summary>
    /// Прямое и обратное преобразование.
    /// </summary>
    public ValueConverter(
        Func<TSource, object, TTarget> convert,
        Func<TTarget, object, TSource> convertBack)
    {
        _convert = convert;
        _convertBack = convertBack;
    }

    #endregion

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (_convert is null)
            throw new NotSupportedException("Конвертация не реализована");

        var src = value is TSource s ? s : default;
        return _convert(src, parameter ?? StaticParameter!);
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (_convertBack is null)
            throw new NotSupportedException("Обратная конвертация не реализована");

        var tgt = value is TTarget t ? t : default;
        return _convertBack(tgt, parameter ?? StaticParameter!);
    }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
