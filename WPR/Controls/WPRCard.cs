using System.Windows;
using System.Windows.Controls;

namespace WPR.Controls
{
    /// <summary> Карточка для отображения контента с тенью </summary>
    public class WPRCard : ContentControl
    {
        static WPRCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRCard), new FrameworkPropertyMetadata(typeof(WPRCard)));
        }


        /// <summary> Жирная тень для всплывающего окна </summary>
        public bool IsPopupShadowStyle
        {
            get => (bool)GetValue(IsPopupShadowStyleProperty);
            set => SetValue(IsPopupShadowStyleProperty, value);
        }
        public static readonly DependencyProperty IsPopupShadowStyleProperty =
            DependencyProperty.Register("IsPopupShadowStyle", typeof(bool), typeof(WPRCard), new PropertyMetadata(false));

        /// <summary> Тень для всплывающего окна</summary>
        public bool IsDialogShadowStyle
        {
            get => (bool)GetValue(IsDialogShadowStyleProperty);
            set => SetValue(IsDialogShadowStyleProperty, value);
        }
        public static readonly DependencyProperty IsDialogShadowStyleProperty =
            DependencyProperty.Register("IsDialogShadowStyle", typeof(bool), typeof(WPRCard), new PropertyMetadata(false));

    }
}
