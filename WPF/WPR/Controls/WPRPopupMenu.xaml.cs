using System.Windows;
using System.Windows.Controls;

namespace WPR.Controls
{
    /// <summary>
    /// Всплывающее меню (попап с кнопкой)
    /// </summary>
    public class WPRPopupMenu : ContentControl
    {
        static WPRPopupMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRPopupMenu), new FrameworkPropertyMetadata(typeof(WPRPopupMenu)));
        }
    }
}
