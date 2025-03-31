using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WPR.Icons;

namespace WPR.Controls
{
    /// <summary>
    /// Декорирование текстбокса или другого контента иконкой слева.
    /// Цвет иконки привязывается к свойству helpers:ButtonHelper.MouseOverButtonBrush контента
    /// </summary>
    [ContentProperty(nameof(Content))]
    public class WPRIconDecorator : Control
    {
        static WPRIconDecorator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRIconDecorator), new FrameworkPropertyMetadata(typeof(WPRIconDecorator)));
        }

        #region Icon : PackIconKind - Иконка

        /// <summary>Иконка</summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(PackIconKind),
                typeof(WPRIconDecorator),
                new PropertyMetadata(default(PackIconKind)));

        /// <summary>Иконка</summary>
        [Category("WPRIconDecorator")]
        [Description("Иконка")]
        public PackIconKind Icon
        {
            get => (PackIconKind) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        #endregion

        #region Content : FrameworkElement - Текстбокс декоратора

        /// <summary>Текстбокс декоратора</summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(FrameworkElement),
                typeof(WPRIconDecorator),
                new PropertyMetadata(default(FrameworkElement)));

        /// <summary>Текстбокс декоратора</summary>
        [Category("WPRIconDecorator")]
        [Description("Текстбокс декоратора")]
        public FrameworkElement Content
        {
            get => (FrameworkElement) GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        #endregion

    }
}
