using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPR.Styles.Services;

namespace WPR.Styles.Styles
{
    partial class ListBoxes
    {
        private void ListBoxItem_Selected(object sender, RoutedEventArgs e) 
        {
            if(sender is not ListBoxItem item) return;
            ColorAnimation animation = new()
            {
                To = Design.GetBrushFromResource( Design.StyleBrush.TextColorBrush).Color,
                Duration = TimeSpan.FromSeconds(0.2),
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.ForegroundProperty));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            storyboard.Begin(item);
        }

        private void ListBoxItem_UnSelected(object sender, RoutedEventArgs e)
        {
            if (sender is not ListBoxItem item) return;
            ColorAnimation animation = new()
            {
                Duration = TimeSpan.FromSeconds(0.2),
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.ForegroundProperty));
            Storyboard storyboard = new();
            storyboard.Children.Add(animation);
            storyboard.Begin(item);
        }
    }
}
