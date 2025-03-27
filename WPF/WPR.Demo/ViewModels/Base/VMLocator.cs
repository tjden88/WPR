using WPR.Demo.Services;

namespace WPR.Demo.ViewModels.Base
{
    internal class VMLocator
    {
        #region MainWindowViewModel : MainWindowViewModel - Модель главного окна

        private MainWindowViewModel _MainWindowViewModel;

        /// <summary>Модель главного окна</summary>
        public MainWindowViewModel MainWindowViewModel
        {
            get => _MainWindowViewModel ??= new MainWindowViewModel(new GetPages());
            set => _MainWindowViewModel = value;
        }

        #endregion

    }
}