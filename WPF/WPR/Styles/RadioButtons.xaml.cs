using System.Windows.Controls;
using WPR.Controls;

namespace WPR.Styles;

partial class RadioButtons
{
    private void WPRRadioButton_MouseDown(object sender, EventArgs e)
    {
        if (sender is not RadioButton radioButton) return;
        var ripple = radioButton.Template.FindName("Ripple", radioButton) as Ripple;
        ripple?.StartRipple();
    }
}