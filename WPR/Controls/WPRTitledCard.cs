using System.Windows;
using System.Windows.Controls;
using WPR.Infrastructure.Icons;

namespace WPR.Controls
{
    /// <summary> Панелька с заголовком и иконкой </summary>
    public class WPRTitledCard : ContentControl
    {
        static WPRTitledCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRTitledCard),
                new FrameworkPropertyMetadata(typeof(WPRTitledCard)));
        }

        /// <summary> Текст заголовка </summary>
        public string Header
        {
            get => (string) GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(WPRTitledCard),
                new PropertyMetadata(string.Empty));


        /// <summary> Значок заголовка </summary>
        public PackIconKind IconSource
        {
            get => (PackIconKind) GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof(PackIconKind), typeof(WPRTitledCard),
                new PropertyMetadata(PackIconKind.InfoCircle));
    }

}
