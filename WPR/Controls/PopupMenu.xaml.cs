using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WPR.Icons;

namespace WPR;

/// <summary>
/// Всплывающее меню (попап с кнопкой)
/// </summary>
public class PopupMenu : ContentControl
{
    static PopupMenu()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupMenu), new FrameworkPropertyMetadata(typeof(PopupMenu)));
    }


    #region Icon : PackIconKind - Значок меню

    /// <summary>Значок меню</summary>
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(PackIconKind),
            typeof(PopupMenu),
            new PropertyMetadata(PackIconKind.DotsVertical));

    /// <summary>Значок меню</summary>
    [Category("PopupMenu")]
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
            typeof(PopupMenu),
            new PropertyMetadata(18d));

    /// <summary>Размер значка</summary>
    [Category("PopupMenu")]
    [Description("Размер значка")]
    public double IconSize
    {
        get => (double) GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    #endregion


}