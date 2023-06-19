using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WPR.Controls.Base;

public abstract class NumericDecorator<T>: ContentControl
{
    #region Value : T - Значение

    /// <summary>Значение</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(T),
            typeof(NumericDecorator<T>),
            new PropertyMetadata(default(T)));

    /// <summary>Значение</summary>
    [Category("NumericDecorator")]
    [Description("Значение")]
    public T Value
    {
        get => (T) GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    #endregion
}