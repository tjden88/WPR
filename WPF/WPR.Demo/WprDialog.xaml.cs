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



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Completed?.Invoke(false);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Completed?.Invoke(true);
        }

        public event Action<bool> Completed;
    }
}
