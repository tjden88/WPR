using System.Windows;
using System.Windows.Controls;

namespace WPR;

/// <summary> Карточка для отображения контента с тенью </summary>
public class Card : ContentControl
{
    static Card()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Card), new FrameworkPropertyMetadata(typeof(Card)));
    }


    /// <summary> Жирная тень для всплывающего окна </summary>
    public bool IsPopupShadowStyle
    {
        get => (bool)GetValue(IsPopupShadowStyleProperty);
        set => SetValue(IsPopupShadowStyleProperty, value);
    }
    public static readonly DependencyProperty IsPopupShadowStyleProperty =
        DependencyProperty.Register(nameof(IsPopupShadowStyle), typeof(bool), typeof(Card), new PropertyMetadata(false));

    /// <summary> Тень для всплывающего окна</summary>
    public bool IsDialogShadowStyle
    {
        get => (bool)GetValue(IsDialogShadowStyleProperty);
        set => SetValue(IsDialogShadowStyleProperty, value);
    }
    public static readonly DependencyProperty IsDialogShadowStyleProperty =
        DependencyProperty.Register(nameof(IsDialogShadowStyle), typeof(bool), typeof(Card), new PropertyMetadata(false));

}