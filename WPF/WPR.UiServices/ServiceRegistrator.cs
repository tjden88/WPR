using Microsoft.Extensions.DependencyInjection;
using WPR.Domain.Interfaces;
using WPR.UiServices.Interfaces;
using WPR.UiServices.Themes;
using WPR.UiServices.UI;

// ReSharper disable once CheckNamespace
namespace WPR.UiServices;

public static class ServiceRegistrator
{

    /// <summary>
    /// Добавить сервисы диалогов, навигации, менеджера тем
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddWPRUiServices(this IServiceCollection services) => services
            .AddSingleton<IUserDialog, UserDialog>()
            .AddSingleton<IAppNavigation, AppNavigationService>()
            .AddSingleton<IColorThemeManager, WPRColorThemeManager>()
            .AddSingleton<IMessageBus, MessageBusService>()
            .AddTransient(typeof(Lazy<>), typeof(LazilyResolved<>))
    ;
}