using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using WPR.Demo.Commands.Base;
using WPR.Demo.Services;
using WPR.Demo.ViewModels.Base;
using WPR;

namespace WPR.Demo.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Title : string - Заголовок

        private string _Title = "WPR..Demo";

        /// <summary>Заголовок</summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        #endregion

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
            Pages = ServiceLocator.PageService.GetAllPages();
            SelectedPage = Pages.First();
        }
    }
}
