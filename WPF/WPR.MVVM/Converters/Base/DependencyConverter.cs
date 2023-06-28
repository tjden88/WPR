using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace WPR.MVVM.Converters.Base;

/// <summary> Конвертер с возможостью привязки другого конвертера </summary>
public class DependencyConverter : Freezable, IValueConverter
{
    #region Converter : IValueConverter - Конвертер

    /// <summary>Конвертер</summary>
    public static readonly DependencyProperty ConverterProperty =
        DependencyProperty.Register(
            nameof(Converter),
            typeof(IValueConverter),
            typeof(DependencyConverter),
            new PropertyMetadata(default(IValueConverter)));

    /// <summary>Конвертер</summary>
    [Category("DependencyConverter")]
    [Description("Конвертер")]
    [ConstructorArgument(nameof(Converter))]
    public IValueConverter Converter
    {
        get => (IValueConverter) GetValue(ConverterProperty);
        set => SetValue(ConverterProperty, value);
    }

    #endregion


    #region ConverterParameter : object - Параметр конвертера

    /// <summary>Параметр конвертера</summary>
    public static readonly DependencyProperty ConverterParameterProperty =
        DependencyProperty.Register(
            nameof(ConverterParameter),
            typeof(object),
            typeof(DependencyConverter),
            new PropertyMetadata(default(object)));

    /// <summary>Параметр конвертера</summary>
    [Category("DependencyConverter")]
    [Description("Параметр конвертера")]
    public object ConverterParameter
    {
        get => (object) GetValue(ConverterParameterProperty);
        set => SetValue(ConverterParameterProperty, value);
    }

    #endregion

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Converter?.Convert(value, targetType, ConverterParameter ?? parameter, culture);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Converter?.ConvertBack(value, targetType, ConverterParameter ?? parameter, culture);

    protected override Freezable CreateInstanceCore() => this;
}