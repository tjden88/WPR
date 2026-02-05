using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using WPR.Tools.Attributes;

namespace WPR.Tools.Extensions;

/// <summary>
/// Методы расширения для регистрации реализаций сервисов из сборки.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует все реализации <typeparamref name="TService"/> из указанной сборки
    /// с заданным временем жизни.
    /// Реализации сортируются по <see cref="ServiceOrderAttribute"/> (по умолчанию 0).
    /// </summary>
    /// <typeparam name="TService">Интерфейс или базовый тип сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="assembly">Сборка, в которой искать реализации.</param>
    /// <param name="lifetime">Время жизни сервиса.</param>
    /// <param name="addInterfaceOnly">
    /// Если true — регистрирует только как <typeparamref name="TService"/>.
    /// Если false — регистрирует и конкретный тип, и интерфейс.
    /// </param>
    /// <returns>Та же коллекция сервисов.</returns>
    public static IServiceCollection AddFromAssembly<TService>(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime, bool addInterfaceOnly = false)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        var serviceType = typeof(TService);

        var implementations = assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } && serviceType.IsAssignableFrom(t))
            .Select(t => new
            {
                Type = t,
                Order = t.GetCustomAttribute<ServiceOrderAttribute>()?.Order ?? 0
            })
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Type.FullName, StringComparer.Ordinal)
            .ToArray();

        foreach (var impl in implementations)
        {
            if (addInterfaceOnly)
            {
                services.Add(new ServiceDescriptor(serviceType, impl.Type, lifetime));
            }
            else
            {
                // Регистрируем конкретный тип
                services.Add(new ServiceDescriptor(impl.Type, impl.Type, lifetime));

                // И интерфейс, с тем же экземпляром
                services.Add(new ServiceDescriptor(serviceType, sp => (TService)sp.GetRequiredService(impl.Type), lifetime));
            }
        }

        return services;
    }

    /// <summary>
    /// Регистрирует все реализации <typeparamref name="TService"/> из указанной сборки
    /// Реализации сортируются по <see cref="ServiceOrderAttribute"/> (по умолчанию 0).
    /// </summary>
    /// <typeparam name="TService">Интерфейс или базовый тип сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="assembly">Сборка, в которой искать реализации.</param>
    /// <param name="addInterfaceOnly">
    /// Если true — регистрирует только как <typeparamref name="TService"/>.
    /// Если false — регистрирует и конкретный тип, и интерфейс.
    /// </param>
    /// <returns>Та же коллекция сервисов.</returns>
    public static IServiceCollection AddFromAssemblySingleton<TService>(this IServiceCollection services, Assembly assembly, bool addInterfaceOnly = false)
        where TService : class => services.AddFromAssembly<TService>(assembly, ServiceLifetime.Singleton, addInterfaceOnly);

    /// <summary>
    /// Регистрирует все реализации <typeparamref name="TService"/> из указанной сборки
    /// Реализации сортируются по <see cref="ServiceOrderAttribute"/> (по умолчанию 0).
    /// </summary>
    /// <typeparam name="TService">Интерфейс или базовый тип сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="assembly">Сборка, в которой искать реализации.</param>
    /// <param name="addInterfaceOnly">
    /// Если true — регистрирует только как <typeparamref name="TService"/>.
    /// Если false — регистрирует и конкретный тип, и интерфейс.
    /// </param>
    /// <returns>Та же коллекция сервисов.</returns>
    public static IServiceCollection AddFromAssemblyScoped<TService>(this IServiceCollection services, Assembly assembly, bool addInterfaceOnly = false)
        where TService : class => services.AddFromAssembly<TService>(assembly, ServiceLifetime.Scoped, addInterfaceOnly);

    /// <summary>
    /// Регистрирует все реализации <typeparamref name="TService"/> из указанной сборки
    /// Реализации сортируются по <see cref="ServiceOrderAttribute"/> (по умолчанию 0).
    /// </summary>
    /// <typeparam name="TService">Интерфейс или базовый тип сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="assembly">Сборка, в которой искать реализации.</param>
    /// <param name="addInterfaceOnly">
    /// Если true — регистрирует только как <typeparamref name="TService"/>.
    /// Если false — регистрирует и конкретный тип, и интерфейс.
    /// </param>
    /// <returns>Та же коллекция сервисов.</returns>
    public static IServiceCollection AddFromAssemblyTransient<TService>(this IServiceCollection services, Assembly assembly, bool addInterfaceOnly = false)
        where TService : class => services.AddFromAssembly<TService>(assembly, ServiceLifetime.Transient, addInterfaceOnly);
}