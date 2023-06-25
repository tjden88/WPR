using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WPR.Icons;

namespace WPR.Controls;

/// <summary>
/// Всплывающее меню (попап с кнопкой)
/// </summary>
public class WPRPopupMenu : ContentControl
{
    static WPRPopupMenu()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRPopupMenu), new FrameworkPropertyMetadata(typeof(WPRPopupMenu)));
    }


    #region Icon : PackIconKind - Значок меню

    /// <summary>Значок меню</summary>
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(PackIconKind),
            typeof(WPRPopupMenu),
            new PropertyMetadata(PackIconKind.DotsVertical));

    /// <summary>Значок меню</summary>
    [Category("WPRPopupMenu")]
    [Description("Значок меню")]
    public PackIconKind Icon
    {
        get => (PackIconKind) GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    #endregion  


}