using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPR.Styles;

partial class Windows
{
    private void Close_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button b && Window.GetWindow(b) is { } parentWindow) parentWindow.Close();
    }

    private void MaximizeRestoreClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button b && Window.GetWindow(b) is { } parentWindow)
            parentWindow.WindowState = parentWindow.WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal;
    }

    private void MinimizeClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button b && Window.GetWindow(b) is { } parentWindow)
            parentWindow.WindowState = WindowState.Minimized;
    }

    private void ModalWindow_MouseMove(object Sender, MouseEventArgs E)
    {
        if (E.LeftButton == MouseButtonState.Pressed) (Sender as Window)?.DragMove();
    }

    private void ModalWindow_Loaded(object Sender, RoutedEventArgs E)
    {
        if (Sender is Window w) w.Closing += ModalWindowOnClosing;
    }

    private static void ModalWindowOnClosing(object Sender, CancelEventArgs E)
    {
        if (Sender is Window w && w.Template?.FindName("Transform", w) is ScaleTransform {ScaleX: 1.0} sc)
        {
            E.Cancel = true;
            w.CacheMode = new BitmapCache();
            DoubleAnimation a = new()
            {
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new ElasticEase() {Oscillations = 1, EasingMode = EasingMode.EaseIn}
            };
            a.Completed += (_, _) => w.Close();
            sc.BeginAnimation(ScaleTransform.ScaleXProperty, a);
        }
    }
}