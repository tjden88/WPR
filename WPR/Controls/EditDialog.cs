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
    /// Панелька с заголовком и иконкой
    /// </summary>
    public class EditDialog : ContentControl
    {
        static EditDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditDialog),
                new FrameworkPropertyMetadata(typeof(EditDialog)));
        }

        /// <summary>
        /// Текст заголовка
        /// </summary>
        public string Header
        {
            get => (string) GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(EditDialog),
                new PropertyMetadata(string.Empty));


        /// <summary>
        /// Значок заголовка
        /// </summary>
        public Icons.PackIconKind IconSource
        {
            get => (Icons.PackIconKind) GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof(Icons.PackIconKind), typeof(EditDialog),
                new PropertyMetadata(Icons.PackIconKind.InfoCircle));
    }

}
