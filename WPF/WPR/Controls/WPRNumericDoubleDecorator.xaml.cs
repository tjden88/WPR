using System.ComponentModel;
using System;
using System.Windows;
using WPR.Controls.Base;

namespace WPR.Controls;

public class WPRNumericDoubleDecorator : NumericDecorator<double>
{
    static WPRNumericDoubleDecorator()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRNumericDoubleDecorator), new FrameworkPropertyMetadata(typeof(WPRNumericDoubleDecorator)));
    }

    

    public WPRNumericDoubleDecorator() : base(1d, double.MinValue, double.MaxValue)
    {
    }

    #region DecimalPlaces : int - Количество десятичных знаков

    /// <summary>Количество десятичных знаков</summary>
    public static readonly DependencyProperty DecimalPlacesProperty =
        DependencyProperty.Register(
            nameof(DecimalPlaces),
            typeof(int),
            typeof(WPRNumericDoubleDecorator),
            new PropertyMetadata(15, null, (d, BaseValue) =>
                Math.Max(0, Math.Min(15, (int)BaseValue))));

    /// <summary>Количество десятичных знаков</summary>
    [Category("Настройки")]
    [Description("Количество десятичных знаков")]
    public int DecimalPlaces
    {
        get => (int)GetValue(DecimalPlacesProperty);
        set => SetValue(DecimalPlacesProperty, value);
    }

    #endregion

    protected override void OnIncrementValueCommandExecuted()
    {
        throw new NotImplementedException();
    }

    protected override void OnDecrementValueCommandExecuted()
    {
        throw new NotImplementedException();
    }

    protected override double ParseValue(string TextValue)
    {
        throw new NotImplementedException();
    }

    protected override double CoerseValue(double baseValue)
    {
        throw new NotImplementedException();
    }

    protected override string SetText(double value)
    {
        throw new NotImplementedException();
    }

    protected override double CalculateFromStringExpression(string Expression)
    {
        throw new NotImplementedException();
    }
}