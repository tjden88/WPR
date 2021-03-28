using System.Windows;
using System.Windows.Controls;

namespace WPR.Styles.Styles
{
    partial class ChromeWindow
    {
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && Window.GetWindow(b) is Window parentWindow) parentWindow.Close();
        }

        private void MaximizeRestoreClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && Window.GetWindow(b) is Window parentWindow)
                parentWindow.WindowState = parentWindow.WindowState == WindowState.Normal
                    ? WindowState.Maximized
                    : WindowState.Normal;
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && Window.GetWindow(b) is Window parentWindow)
                parentWindow.WindowState = WindowState.Minimized;
        }
    }
}
