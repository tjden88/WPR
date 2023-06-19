using System.Windows;
using WPR.Controls.Base;

namespace WPR.Controls;

public class WPRNumericIntDecorator : NumericDecorator<int>
{
    static WPRNumericIntDecorator()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRNumericIntDecorator), new FrameworkPropertyMetadata(typeof(WPRNumericIntDecorator)));
    }

}