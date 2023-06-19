using System.Windows;
using WPR.Controls.Base;

namespace WPR.Controls;

public class WPRNumericDoubleDecorator : NumericDecorator<double>
{
    static WPRNumericDoubleDecorator()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRNumericDoubleDecorator), new FrameworkPropertyMetadata(typeof(WPRNumericDoubleDecorator)));
    }
}