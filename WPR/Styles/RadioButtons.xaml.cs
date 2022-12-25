using System;
using System.Windows.Controls;
using WPR.Controls;

namespace WPR.Styles;

partial class RadioButtons
{
    void WPRRadioButton_MouseDown(object sender, EventArgs e)
    {
        if (sender is not RadioButton radioButton) return;
        Ripple ripple = radioButton.Template.FindName("Ripple", radioButton) as Ripple;
        ripple?.StartRipple();
    }
}