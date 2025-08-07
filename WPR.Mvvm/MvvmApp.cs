using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;
using WPR.Mvvm.Interfaces;
using WPR.Mvvm.Services;

namespace WPR.Mvvm;

/// <summary>
/// Приложение с навигацией и автоматическим биндингом моделей-представлений
/// </summary>
public abstract class MvvmApp : Application
{
    private static readonly ViewsRegistrator _ViewsRegistrator = new(ServiceResolver.ServiceCollection);


    /// <summary> Коллекция сервисов приложения </summary>
    protected static IServiceProvider Services => ServiceResolver.Services;


    protected override void OnStartup(StartupEventArgs e)
    {
        var ci = CultureInfo.GetCultureInfo("RU");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(ci.Name)));

        base.OnStartup(e);
        var services = ServiceResolver.ServiceCollection;
        ConfigureServices(services);

        // Регистрация собственных сервисов в контейнере
        services.AddSingleton(typeof(IResolver<>), typeof(Resolver<>));

        RegisterViews(_ViewsRegistrator);
        Resources.MergedDictionaries.Add(_ViewsRegistrator.ResourceDictionary);
        ServiceResolver.IsAllServiceAdded = true;
    }


    /// <summary>
    /// Конфигурация сервисов приложения в контейнер сервисов
    /// </summary>
    protected abstract void ConfigureServices(IServiceCollection services);

    /// <summary>
    /// Регистрация представлений и связанных с ними моделей-представлений в контейнер сервисов
    /// </summary>
    /// <param name="ViewsRegistrator"></param>
    protected abstract void RegisterViews(IViewsRegistrator ViewsRegistrator);

}