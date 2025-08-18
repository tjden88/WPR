using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPR.Demo.Services.Interfaces;
using WPR.Mvvm.ViewModels;
using WPR.Theme;

namespace WPR.Demo.ViewModels
{
    internal partial class MainWindowViewModel : ViewModel
    {

        [RelayCommand]
        private void SetNewStyle()
        {
            StyleHelper.SetNewRandomStyle();
        }

        [RelayCommand]

        private void SetDarkTheme()
        {
            StyleHelper.SetColorScheme(StyleHelper.IsDarkTheme ? ColorScheme.Light : ColorScheme.Dark);
        }

        [RelayCommand]

        private void SetSystemTheme()
        {
            StyleHelper.SetColorScheme(ColorScheme.Auto);
        }


        [RelayCommand]

        private void ShowTestWindow()
        {
            EmptyWindow w = new EmptyWindow();
            w.Show();
        }



        [ObservableProperty]
        private Page _SelectedPage;


        [ObservableProperty]
        private IEnumerable<Page> _Pages;


        public MainWindowViewModel(IGetPages GetPages)
        {
            Pages = GetPages.GetAllPages();
            SelectedPage = Pages.First();
        }
    }
}
