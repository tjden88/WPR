using System.ComponentModel;
using System;
using System.Globalization;
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

    protected override double IncrementValue() => Math.Min(MaxValue, Value + Increment);

    protected override double DecrementValue() => Math.Max(MinValue, Value - Increment);

    protected override double ParseValue(string TextValue) => Math.Round(TextValue.ConvertToDouble(), DecimalPlaces);

    protected override double CoerseValue(double baseValue, out string errorText)
    {
        errorText = null;

        if (baseValue < MinValue)
            errorText = $"Минимальное значение: {MinValue}";

        if (baseValue > MaxValue)
            errorText = $"Максимальное значение: {MaxValue}";

        return Math.Round(Math.Clamp(baseValue, MinValue, MaxValue), DecimalPlaces);
    }

    protected override string SetText(double value) => value.ToString(CultureInfo.InvariantCulture);

    protected override double CalculateFromStringExpression(string Expression, out string errorText)
    {
        var expressionIsValid = Expression.CalculateStringExpression(out var result, DecimalPlaces);

        errorText = expressionIsValid ? null : "Неверное выражение";

        return expressionIsValid ? result : 0d;
    }
}