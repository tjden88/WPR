using Microsoft.Extensions.DependencyInjection;

namespace WPR.Mvvm;

/// <summary>
/// Хранение сервисов приложения
/// </summary>
internal static class ServiceResolver
{
    /// <summary> Установить в True, когда все сервисы будут добавлены в коллекцию. Если доступ произойдёт раньше, будет исключение </summary>
    internal static bool IsAllServiceAdded { get; set; }

    /// <summary>
    /// Добавить сюда сервисы перед вызовом построителя
    /// </summary>
    internal static readonly ServiceCollection ServiceCollection = new();


    private static IServiceProvider? _Services;

    /// <summary> Коллекция сервисов приложения </summary>
    internal static IServiceProvider Services => _Services ??= ConfigureServices();


    // Конфигурация внутренних сервисов WPF приложений
    private static IServiceProvider ConfigureServices()
    {
        if (!IsAllServiceAdded)
            throw new InvalidOperationException("Попытка получить сервисы до разрешения");

        return ServiceCollection.BuildServiceProvider();
    }
}