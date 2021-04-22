using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPR.Styles
{
    partial class TabControls
    {
        private static readonly Storyboard RectAnim = new ();
        private static readonly Storyboard ContentAnim = new ();
        private static readonly ThicknessAnimation Position = new() { Duration = TimeSpan.FromSeconds(0.5), EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut } };
        private static readonly DoubleAnimation Size = new () { Duration = TimeSpan.FromSeconds(0.5), EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut } };
        private static readonly DoubleAnimation Opacity = new (0, 1, TimeSpan.FromSeconds(0.4));
        private static readonly DoubleAnimation ScaleX = new (-50, 0, TimeSpan.FromSeconds(0.4)) { EasingFunction= new CircleEase() { EasingMode = EasingMode.EaseOut } };

        static TabControls()
        {
            RectAnim.Children.Add(Position);
            RectAnim.Children.Add(Size);

            Storyboard.SetTargetProperty(Position, new PropertyPath(FrameworkElement.MarginProperty));
            Storyboard.SetTargetProperty(Size, new PropertyPath(FrameworkElement.WidthProperty));

            ContentAnim.Children.Add(Opacity);
            ContentAnim.Children.Add(ScaleX);
            Storyboard.SetTargetProperty(Opacity, new PropertyPath(UIElement.OpacityProperty));
            Storyboard.SetTargetProperty(ScaleX, new PropertyPath("RenderTransform.X"));

        }

        private void Tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is TabControl tabControl)
            {
                SetRectPos(tabControl);
                if (tabControl.Template.FindName("PART_SelectedContentHost", tabControl) is ContentPresenter contentPresenter)
                {
                    ContentAnim.Begin(contentPresenter);
                }
            }
        }

        private void Tab_Loaded(object sender, RoutedEventArgs e)
        {
            SetRectPos(sender as TabControl);
        }

        private void Tab_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((sender as TabItem)?.Parent is TabControl tab) SetRectPos(tab);
        }

        private static void SetRectPos (TabControl tab)
        {
            if (tab.Template?.FindName("Rect", tab) is Rectangle rect)
            {
                if (tab.SelectedItem is TabItem item)
                {
                    Position.To = new Thickness(item.TranslatePoint(new Point(), tab).X, 0, 0, 1);
                    Size.To = item.ActualWidth;
                    RectAnim.Begin(rect);
                }
                else
                {
                    rect.Width = 0;
                    rect.Margin = new Thickness(0, 0, 0, 1);
                }
            }

        }
    }
}
