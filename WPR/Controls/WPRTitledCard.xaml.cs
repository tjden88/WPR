using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WPR.Icons;

namespace WPR.Controls;

/// <summary> Панелька с заголовком и иконкой </summary>
public class WPRTitledCard : ContentControl
{
    static WPRTitledCard()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRTitledCard),
            new FrameworkPropertyMetadata(typeof(WPRTitledCard)));
    }

    /// <summary> Текст заголовка </summary>
    public string Header
    {
        get => (string) GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
    [Category("WPRTitledCard")]
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(string), typeof(WPRTitledCard),
            new PropertyMetadata(string.Empty));


    /// <summary> Значок заголовка </summary>
    public PackIconKind IconSource
    {
        get => (PackIconKind) GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    // Using a DependencyProperty as the backing store for IconSource.  This enables animation, styling, binding, etc...
    [Category("WPRTitledCard")]
    public static readonly DependencyProperty IconSourceProperty =
        DependencyProperty.Register("IconSource", typeof(PackIconKind), typeof(WPRTitledCard),
            new PropertyMetadata(PackIconKind.InfoCircle));

    #region PopupMenu : Menu - Меню в правой части заголовка

    /// <summary>Меню в правой части заголовка</summary>
    public static readonly DependencyProperty PopupMenuProperty =
        DependencyProperty.Register(
            nameof(PopupMenu),
            typeof(Menu),
            typeof(WPRTitledCard),
            new PropertyMetadata(default(Menu)));

    /// <summary>Меню в правой части заголовка</summary>
    [Category("WPRTitledCard")]
    [Description("Меню в правой части заголовка")]
    public Menu PopupMenu
    {
        get => (Menu) GetValue(PopupMenuProperty);
        set => SetValue(PopupMenuProperty, value);
    }

    #endregion

    #region ShowMenuButton : bool - Показывать кнопку всплывающего меню

    /// <summary>Показывать кнопку всплывающего меню</summary>
    public static readonly DependencyProperty ShowMenuButtonProperty =
        DependencyProperty.Register(
            nameof(ShowMenuButton),
            typeof(bool),
            typeof(WPRTitledCard),
            new PropertyMetadata(default(bool)));

    /// <summary>Показывать кнопку всплывающего меню</summary>
    [Category("WPRTitledCard")]
    [Description("Показывать кнопку всплывающего меню")]
    public bool ShowMenuButton
    {
        get => (bool) GetValue(ShowMenuButtonProperty);
        set => SetValue(ShowMenuButtonProperty, value);
    }

    #endregion
}