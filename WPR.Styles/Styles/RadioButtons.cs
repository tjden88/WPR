using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPR.Styles.Controls;

namespace WPR.Styles.Styles
{
    partial class RadioButtons
    {
        void WPRRadioButton_MouseDown(object sender, EventArgs e)
        {
            if (sender is not RadioButton radioButton) return;
            Ripple ripple = radioButton.Template.FindName("Ripple", radioButton) as Ripple;
            ripple?.StartRipple();
        }
    }
}
