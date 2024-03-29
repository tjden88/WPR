﻿using System;
using System.Windows.Controls;
using WPR.Controls;

namespace WPR.Styles;

partial class CheckBoxes
{
    private void WPRCheckBox_MouseUp(object sender, EventArgs e)
    {
        if (sender is not CheckBox checkBox) return;
        var ripple = checkBox.Template.FindName("Ripple", checkBox) as Ripple;
        ripple?.StartRipple();
    }
}