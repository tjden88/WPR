namespace WPR.Demo.ViewModels.Base
{
    internal class VmLocator
    {
        public MainWindowViewModel MainWindowViewModel => App.GetService<MainWindowViewModel>();
        public TestViewModel TestViewModel => App.GetService<TestViewModel>();
    }
}
