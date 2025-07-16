using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WPR;

/// <summary>
/// Текст в овальной рамке - появляется при наведении на дочерний элемент
/// </summary>
public class WPRTipDecorator : ContentControl
{
    static WPRTipDecorator()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRTipDecorator), new FrameworkPropertyMetadata(typeof(WPRTipDecorator)));
    }

    #region Tip : object - Контент подсказки при наведении

    /// <summary>Контент подсказки при наведении</summary>
    public static readonly DependencyProperty TipProperty =
        DependencyProperty.Register(
            nameof(Tip),
            typeof(object),
            typeof(WPRTipDecorator),
            new PropertyMetadata(default(object)));

    /// <summary>Контент подсказки при наведении</summary>
    [Category("WPRTipDecorator")]
    [Description("Контент подсказки при наведени")]
    public object Tip
    {
        get => GetValue(TipProperty);
        set => SetValue(TipProperty, value);
    }

    #endregion


    #region TipMargin : Thickness - Отступ контента подсказки

    /// <summary>Отступ контента подсказки</summary>
    public static readonly DependencyProperty TipMarginProperty =
        DependencyProperty.Register(
            nameof(TipMargin),
            typeof(Thickness),
            typeof(WPRTipDecorator),
            new PropertyMetadata(default(Thickness)));

    /// <summary>Отступ контента подсказки</summary>
    [Category("WPRTipDecorator")]
    [Description("Отступ контента подсказки")]
    public Thickness TipMargin
    {
        get => (Thickness) GetValue(TipMarginProperty);
        set => SetValue(TipMarginProperty, value);
    }

    #endregion
}