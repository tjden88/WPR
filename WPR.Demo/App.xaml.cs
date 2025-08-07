using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WPR.Demo.Services;
using WPR.Demo.Services.Interfaces;
using WPR.Demo.ViewModels;
using WPR.Mvvm.Interfaces;

namespace WPR.Demo;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var window = Services.GetRequiredService<MainWindow>();
        window.Show();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IGetPages, GetPages>();
    }

    protected override void RegisterViews(IViewsRegistrator ViewsRegistrator)
    {
        ViewsRegistrator
            .AddMainWindow<MainWindow, MainWindowViewModel>()

            ;
    }
}