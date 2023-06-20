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

    protected override void OnIncrementValueCommandExecuted()
    {
        Value += Increment;
    }

    protected override void OnDecrementValueCommandExecuted()
    {
        Value -= Increment;
    }

    protected override int ParseValue(string TextValue)
    {
        return TextValue.ConvertToInt();
    }

    protected override int CoerseValue(int baseValue)
    {
        return Math.Clamp(baseValue, MinValue, MaxValue);
    }

    protected override string SetText(int value)
    {
        return value.ToString();
    }

    protected override int CalculateFromStringExpression(string Expression)
    {
        var expressionIsValid = Expression.CalculateStringExpression(out var result, 0);
        return expressionIsValid ? (int) result : 0;
    }
}