using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using WPR.Demo.Commands.Base;
using WPR.Demo.Services;
using WPR.Demo.ViewModels.Base;
using WPR;
using WPR.MVVM.ViewModels;

namespace WPR.Demo.ViewModels
{
    internal class MainWindowViewModel : WindowViewModel
    {

        #region Command SetNewStyleCommand - Установить рандомный стиль

        private ICommand _SetNewStyleCommand;

        /// <summary>Установить рандомный стиль</summary>
        public ICommand SetNewStyleCommand => _SetNewStyleCommand
            ??= new Command(OnSetNewStyleCommandExecuted);

        private void OnSetNewStyleCommandExecuted()
        {
            Design.SetNewRandomStyle();
        }
        #endregion

        #region Command SetDarkThemeCommand - Установить тёмную тему

        private ICommand _SetDarkThemeCommand;

        /// <summary>Установить тёмную тему</summary>
        public ICommand SetDarkThemeCommand => _SetDarkThemeCommand
            ??= new Command(OnSetDarkThemeCommandExecuted);

        private void OnSetDarkThemeCommandExecuted()
        {
            if (Design.IsDarkThemeCurrent)
                Design.SetLightColorTheme();
            else
                Design.SetDarkColorTheme();
        }
        #endregion

        #region Command ShowTestWindowCommand - Показать тестовое окно

        private ICommand _ShowTestWindowCommand;

        /// <summary>Показать тестовое окно</summary>
        public ICommand ShowTestWindowCommand => _ShowTestWindowCommand
            ??= new Command(OnShowTestWindowCommandExecuted, CanShowTestWindowCommandExecute);

        private bool CanShowTestWindowCommandExecute() => true;

        private void OnShowTestWindowCommandExecuted()
        {
            Test test = new();
            test.Show();
        }

        #endregion

        #region SelectedPage : Page - Выбранная страница для отображения

        private Page _SelectedPage;

        /// <summary>Выбранная страница для отображения</summary>
        public Page SelectedPage
        {
            get => _SelectedPage;
            set => Set(ref _SelectedPage, value);
        }
        #endregion

        #region Pages : IEnumerable<Page> - Список страниц

        private IEnumerable<Page> _Pages;

        /// <summary>Список страниц</summary>
        public IEnumerable<Page> Pages
        {
            get => _Pages;
            set => Set(ref _Pages, value);
        }

        #endregion

        public MainWindowViewModel()
        {
            Title = "WPR.Demo";
            Pages = ServiceLocator.PageService.GetAllPages();
            SelectedPage = Pages.First();
        }
    }
}
