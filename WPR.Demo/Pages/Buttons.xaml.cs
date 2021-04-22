using System.Windows;
using System.Windows.Controls;

namespace WPR.Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для Buttons.xaml
    /// </summary>
    public partial class Buttons : Page
    {
        public Buttons()
        {
            InitializeComponent();
        }

        private void Button_Click(object Sender, RoutedEventArgs E)
        {
            if (BageButton.BageVisible == Visibility.Hidden)
            {
                BageButton.BageContent = "1";
                BageButton.BageVisible = Visibility.Visible;
            }
            else
            {
                BageButton.BageContent = (int.Parse((string)BageButton.BageContent) + 1).ToString();
            }
        }
    }
}
