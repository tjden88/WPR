namespace WPR.Demo.ViewModels.Base
{
    internal class VMLocator
    {
        #region MainWindowViewModel : MainWindowViewModel - Модель главного окна

        private MainWindowViewModel _MainWindowViewModel;

        /// <summary>Модель главного окна</summary>
        public MainWindowViewModel MainWindowViewModel
        {
            get => _MainWindowViewModel ??= new MainWindowViewModel();
            set => _MainWindowViewModel = value;
        }

        #endregion

        #region TestViewModel : TestViewModel - Для тестов вьюмодель

        private TestViewModel _TestViewModel;

        /// <summary>Для тестов вьюмодель</summary>
        public TestViewModel TestViewModel
        {
            get => _TestViewModel ??= new();
            set => _TestViewModel = value;
        }

        #endregion
    }
}
