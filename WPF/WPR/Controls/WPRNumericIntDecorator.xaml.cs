using System;
using System.Windows;
using WPR.Controls.Base;

namespace WPR.Controls;

public class WPRNumericIntDecorator : NumericDecorator<int>
{

    static WPRNumericIntDecorator()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRNumericIntDecorator), new FrameworkPropertyMetadata(typeof(WPRNumericIntDecorator)));
    }

    public WPRNumericIntDecorator() : base(1, int.MinValue, int.MaxValue)
    {
    }

    protected override int IncrementValue() => Math.Min(MaxValue, Value + Increment);

    protected override int DecrementValue() => Math.Max(MinValue, Value - Increment);

    protected override int ParseValue(string TextValue) => TextValue.ConvertToInt();

    protected override int CoerseValue(int baseValue, out string errorText)
    {
        errorText = null;

        if (baseValue < MinValue)
            errorText = $"Минимальное значение: {MinValue}";

        if (baseValue > MaxValue)
            errorText = $"Максимальное значение: {MaxValue}";

        return Math.Clamp(baseValue, MinValue, MaxValue);
    }

    protected override string SetText(int value) => value.ToString();

    protected override int CalculateFromStringExpression(string Expression, out string errorText)
    {
        var expressionIsValid = Expression.CalculateStringExpression(out var result, 0);

        errorText = expressionIsValid ? null : "Неверное выражение";

        return expressionIsValid ? (int)result : 0;
    }
}