using System;
using System.Windows;
using System.Windows.Controls;
using WPR.Extensions;

namespace WPR.Styles
{
    partial class ListBoxes
    {
        private void ListBoxItem_Selected(object sender, RoutedEventArgs e) 
        {
            if(sender is not ListBoxItem item) return;
            BrushAnimation animation = new()
            {
                To = Design.GetBrushFromResource( Design.StyleBrush.TextColorBrush),
                Duration = TimeSpan.FromSeconds(0.2),
            };
            item.BeginAnimation(Control.ForegroundProperty, animation);
        }

        private void ListBoxItem_UnSelected(object sender, RoutedEventArgs e)
        {
            if (sender is not ListBoxItem item) return;
            BrushAnimation animation = new()
            {
                Duration = TimeSpan.FromSeconds(0.2),
            };
            item.BeginAnimation(Control.ForegroundProperty, animation);
        }
    }
}
