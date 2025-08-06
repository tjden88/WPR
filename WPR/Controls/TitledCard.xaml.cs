using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WPR.Icons;

namespace WPR;

/// <summary> Панелька с заголовком и иконкой </summary>
public class TitledCard : ContentControl
{
    static TitledCard()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TitledCard),
            new FrameworkPropertyMetadata(typeof(TitledCard)));
    }

    /// <summary> Текст заголовка </summary>
    public string Header
    {
        get => (string) GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
    [Category("TitledCard")]
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(TitledCard),
            new PropertyMetadata(string.Empty));


    /// <summary> Значок заголовка </summary>
    public PackIconKind IconSource
    {
        get => (PackIconKind) GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    // Using a DependencyProperty as the backing store for IconSource.  This enables animation, styling, binding, etc...
    [Category("TitledCard")]
    public static readonly DependencyProperty IconSourceProperty =
        DependencyProperty.Register(nameof(IconSource), typeof(PackIconKind), typeof(TitledCard),
            new PropertyMetadata(PackIconKind.InfoCircle));

    #region PopupMenu : Menu - Меню в правой части заголовка

    /// <summary>Меню в правой части заголовка</summary>
    public static readonly DependencyProperty PopupMenuProperty =
        DependencyProperty.Register(
            nameof(PopupMenu),
            typeof(Menu),
            typeof(TitledCard),
            new PropertyMetadata(default(Menu)));

    /// <summary>Меню в правой части заголовка</summary>
    [Category("TitledCard")]
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
            typeof(TitledCard),
            new PropertyMetadata(default(bool)));

    /// <summary>Показывать кнопку всплывающего меню</summary>
    [Category("TitledCard")]
    [Description("Показывать кнопку всплывающего меню")]
    public bool ShowMenuButton
    {
        get => (bool) GetValue(ShowMenuButtonProperty);
        set => SetValue(ShowMenuButtonProperty, value);
    }

    #endregion
}