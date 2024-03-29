﻿using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using WPR.Controls;

namespace WPR.Styles;

partial class Toggles
{
    private void WPRToggleButton_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if(sender is not ToggleButton toggleButton) return;
        if(toggleButton.Template.FindName("PART_ellipse", toggleButton) is not Ellipse ellipse) return;

        var maxWidth = Math.Sqrt(toggleButton.ActualHeight * toggleButton.ActualHeight + toggleButton.ActualWidth * toggleButton.ActualWidth) * 2;
        ellipse.Width = maxWidth;

        Canvas.SetLeft(ellipse, e.GetPosition(toggleButton).X - maxWidth / 2);
        Canvas.SetTop(ellipse, e.GetPosition(toggleButton).Y - maxWidth / 2);
    }

    private void WPRSwither_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ToggleButton toggleButton) return;
        if (toggleButton.Template.FindName("Ripple", toggleButton) is not Ripple ripple) return;
        ripple.StartRipple();

    }

}