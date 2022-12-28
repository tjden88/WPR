using System;
using System.Windows;
using System.Windows.Controls;
using WPR.Domain.Interfaces;

namespace WPR.Demo
{
    /// <summary>
    /// Логика взаимодействия для WprDialog.xaml
    /// </summary>
    public partial class WprDialog : UserControl, IWPRDialog
    {

        public WprDialog()
        {
            InitializeComponent();
        }


        public Action<bool> SetDialogResult { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult?.Invoke(false);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SetDialogResult?.Invoke(true);
        }
    }
}
