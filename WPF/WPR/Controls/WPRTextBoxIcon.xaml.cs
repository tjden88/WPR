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
    public class WPRTextBoxIcon : Control
    {
        static WPRTextBoxIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRTextBoxIcon), new FrameworkPropertyMetadata(typeof(WPRTextBoxIcon)));
        }

        #region Icon : PackIconKind - Иконка

        /// <summary>Иконка</summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(PackIconKind),
                typeof(WPRTextBoxIcon),
                new PropertyMetadata(default(PackIconKind)));

        /// <summary>Иконка</summary>
        [Category("WPRTextBoxIcon")]
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
                typeof(WPRTextBoxIcon),
                new PropertyMetadata(default(FrameworkElement)));

        /// <summary>Текстбокс декоратора</summary>
        [Category("WPRTextBoxIcon")]
        [Description("Текстбокс декоратора")]
        public FrameworkElement Content
        {
            get => (FrameworkElement) GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        #endregion

    }
}
