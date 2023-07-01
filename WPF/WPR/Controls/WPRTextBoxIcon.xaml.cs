using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WPR.Icons;

namespace WPR.Controls
{
    /// <summary>
    /// Декорирование текстбокса иконкой слева. Она будет менять цвет вслед за текстбоксом
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

        #region Content : TextBox - Текстбокс декоратора

        /// <summary>Текстбокс декоратора</summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(TextBox),
                typeof(WPRTextBoxIcon),
                new PropertyMetadata(default(TextBox)));

        /// <summary>Текстбокс декоратора</summary>
        [Category("WPRTextBoxIcon")]
        [Description("Текстбокс декоратора")]
        public TextBox Content
        {
            get => (TextBox) GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        #endregion

        
    }
}
