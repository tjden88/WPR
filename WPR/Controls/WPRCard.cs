using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPR.Controls
{
    /// <summary>
    /// Карточка для отображения контента с тенью
    /// </summary>
    public class WPRCard : ContentControl
    {
        static WPRCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRCard), new FrameworkPropertyMetadata(typeof(WPRCard)));
        }


        /// <summary>
        /// Жирная тень для всплывающего окна
        /// </summary>
        public bool IsPopupShadowStyle
        {
            get => (bool)GetValue(IsPopupShadowStyleProperty);
            set => SetValue(IsPopupShadowStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsPopupShadowStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPopupShadowStyleProperty =
            DependencyProperty.Register("IsPopupShadowStyle", typeof(bool), typeof(WPRCard), new PropertyMetadata(false));


    }
}
