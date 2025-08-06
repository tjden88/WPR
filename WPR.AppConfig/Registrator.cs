using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using WPR.AppConfig.Implementations;

namespace WPR.AppConfig;

public static class Registrator
{

    /// <summary>
    /// Добавляет конфигурацию приложения из JSON файла в DI контейнер.
    /// </summary>
    /// <typeparam name="T">Класс настроек приложения</typeparam>
    public static IServiceCollection AddJsonFileAppConfig<T>(this IServiceCollection services, string filePath, JsonSerializerOptions? options = null) where T : class, new()
    {
        var config = new JsonFileConfig<T>(filePath, options);
        return services
            .AddSingleton<IConfig<T>>(config)
            .AddSingleton<T>(sp => sp.GetRequiredService<IConfig<T>>().Config);
    }

    /// <summary>
    /// Добавляет конфигурацию приложения из разных поставщиков настроек приложения.
    /// Пример: json - файлы, конфигурация в памяти и т.д.
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="config">Вызовите ConfigBuilder.Build() для получения объекта IConfigRoot</param>
    public static IServiceCollection AddAppConfig(this IServiceCollection services, IConfigRoot config)
    {
        foreach (var configProvider in config.Providers)
        {
            var type = typeof(IConfig<>).MakeGenericType(configProvider.ConfigType);
            services.AddSingleton(type, configProvider);
            services.AddSingleton(configProvider.ConfigType, sp =>
            {
                var service = sp.GetService(type) as IConfigProvider;
                return service?.Config ?? throw new InvalidOperationException($"Ошибка при извлечении инстанса настроек приложения: {type}");
            });
        }

        return services.AddSingleton(config);
    }
}