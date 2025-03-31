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

    #region IconSize : double - Размер значка

    /// <summary>Размер значка</summary>
    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register(
            nameof(IconSize),
            typeof(double),
            typeof(WPRPopupMenu),
            new PropertyMetadata(18d));

    /// <summary>Размер значка</summary>
    [Category("WPRPopupMenu")]
    [Description("Размер значка")]
    public double IconSize
    {
        get => (double) GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    #endregion


}