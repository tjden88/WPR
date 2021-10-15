using System.Windows;
using WPR.Demo.Services;
using WPR.Demo.Services.Interfaces;
using WPR.Demo.ViewModels;
using WPR.MVVM;

namespace WPR.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary> Получить сервис из контейнера IoC </summary>
        internal static T GetService<T>() where T: class => DependensyInjection.Get<T>();

        protected override void OnStartup(StartupEventArgs e)
        {
            // Регистрация ViewModel в IoC контейнере
            DependensyInjection.Registrator.AddSingleton<MainWindowViewModel>()
                .AddSingleton<TestViewModel>();

            // Регистрация сервисов в IoC контейнере
            DependensyInjection.Registrator.AddSingleton<IGetPages, GetPages>();
        }
    }
}
