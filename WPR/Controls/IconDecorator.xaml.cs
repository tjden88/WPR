using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WPR.Icons;

namespace WPR
{
    /// <summary>
    /// Декорирование текстбокса или другого контента иконкой слева.
    /// Цвет иконки привязывается к свойству wpr:Extra.MouseOverBrush контента
    /// </summary>
    [ContentProperty(nameof(Content))]
    public class IconDecorator : Control
    {
        static IconDecorator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconDecorator), new FrameworkPropertyMetadata(typeof(IconDecorator)));
        }

        #region Icon : PackIconKind - Иконка

        /// <summary>Иконка</summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(PackIconKind),
                typeof(IconDecorator),
                new PropertyMetadata(default(PackIconKind)));

        /// <summary>Иконка</summary>
        [Category("IconDecorator")]
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
                typeof(IconDecorator),
                new PropertyMetadata(default(FrameworkElement)));

        /// <summary>Текстбокс декоратора</summary>
        [Category("IconDecorator")]
        [Description("Текстбокс декоратора")]
        public FrameworkElement Content
        {
            get => (FrameworkElement) GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        #endregion

    }
}
