using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using WPR.Demo.Services.Interfaces;
using WPR.ColorTheme;
using WPR.Domain.ViewModels;
using WPR.MVVM.Commands.Base;

namespace WPR.Demo.ViewModels
{
    internal class MainWindowViewModel :ViewModel
    {

        #region Title : string - Заголовок

        /// <summary>Заголовок</summary>
        private string _Title;

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
            StyleHelper.SetNewRandomStyle();
        }
        #endregion

        #region Command SetDarkThemeCommand - Установить тёмную тему

        private ICommand _SetDarkThemeCommand;

        /// <summary>Установить тёмную тему</summary>
        public ICommand SetDarkThemeCommand => _SetDarkThemeCommand
            ??= new Command(OnSetDarkThemeCommandExecuted);

        private void OnSetDarkThemeCommandExecuted()
        {
            if (StyleHelper.IsDarkTheme)
                StyleHelper.SetLightColorTheme();
            else
                StyleHelper.SetDarkColorTheme();
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
            EmptyWindow w = new EmptyWindow();
            w.Show();

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

        public MainWindowViewModel(IGetPages GetPages)
        {
            Title = "WPR.Demo";
            Pages = GetPages.GetAllPages();
            SelectedPage = Pages.First();
        }
    }
}
