using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WPR;

/// <summary>
/// Текст в овальной рамке - появляется при наведении на дочерний элемент
/// </summary>
public class TipDecorator : ContentControl
{
    static TipDecorator()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TipDecorator), new FrameworkPropertyMetadata(typeof(TipDecorator)));
    }

    #region Tip : object - Контент подсказки при наведении

    /// <summary>Контент подсказки при наведении</summary>
    public static readonly DependencyProperty TipProperty =
        DependencyProperty.Register(
            nameof(Tip),
            typeof(object),
            typeof(TipDecorator),
            new PropertyMetadata(default(object)));

    /// <summary>Контент подсказки при наведении</summary>
    [Category("TipDecorator")]
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
            typeof(TipDecorator),
            new PropertyMetadata(default(Thickness)));

    /// <summary>Отступ контента подсказки</summary>
    [Category("TipDecorator")]
    [Description("Отступ контента подсказки")]
    public Thickness TipMargin
    {
        get => (Thickness) GetValue(TipMarginProperty);
        set => SetValue(TipMarginProperty, value);
    }

    #endregion
}