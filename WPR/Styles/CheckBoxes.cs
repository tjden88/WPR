using System;
using System.Windows.Controls;
using WPR.Controls;

namespace WPR.Styles
{
    partial class CheckBoxes
    {
        void WPRCheckBox_MouseUp(object sender, EventArgs e)
        {
            if (sender is not CheckBox checkBox) return;
            Ripple ripple = checkBox.Template.FindName("Ripple", checkBox) as Ripple;
            ripple?.StartRipple();
        }
    }
}
