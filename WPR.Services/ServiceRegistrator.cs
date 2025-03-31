using WPR.Abstractions.Interfaces;
using WPR.Services.Implementations;


// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegistrator
{

    /// <summary>
    /// Добавить сервисы диалогов, навигации, менеджера тем
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddWPRServices(this IServiceCollection services) => services
            .AddSingleton<IUserDialog, UserDialog>()
            .AddSingleton<IColorThemeManager, WPRColorThemeManager>()
            .AddTransient(typeof(Lazy<>), typeof(LazilyResolved<>))
    ;
}