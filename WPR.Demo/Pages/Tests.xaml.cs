using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WPR.Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для Tests.xaml
    /// </summary>
    [ObservableObject]
    public partial class Tests : Page
    {
        public Tests()
        {
            InitializeComponent();
        }



        [RelayCommand]
        private void Test()
        {
        }

    }
}
