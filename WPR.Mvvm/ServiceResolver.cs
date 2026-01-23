namespace WPR.Mvvm;

/// <summary>
/// Хранилище корневого <see cref="IServiceProvider"/> приложения.
/// </summary>
/// <remarks>
/// Это не "магический контейнер", а просто точка доступа для инфраструктуры (например, инжектора в XAML),
/// где невозможно нормально прокинуть зависимости через конструктор.
///
/// Принцип:
/// - Инициализация выполняется один раз в <see cref="Application.OnStartup"/>.
/// - До инициализации доступ запрещён (будет понятное исключение).
/// - На выходе приложения provider должен быть освобождён (Dispose), если поддерживается.
/// </remarks>
internal static class ServiceResolver
{
    private static IServiceProvider? _services;

    /// <summary>
    /// Корневой провайдер сервисов приложения.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Бросается, если попытаться получить сервисы до инициализации.
    /// </exception>
    internal static IServiceProvider Services =>
        _services ?? throw new InvalidOperationException(
            "Сервисы приложения ещё не инициализированы. " +
            "Проверь, что ServiceResolver.Initialize(...) вызывается в OnStartup до любого обращения к Services.");

    /// <summary>
    /// Признак, что провайдер уже инициализирован.
    /// </summary>
    internal static bool IsInitialized => _services is not null;

    /// <summary>
    /// Инициализирует корневой провайдер сервисов приложения.
    /// </summary>
    /// <param name="serviceProvider">Готовый <see cref="IServiceProvider"/>.</param>
    /// <exception cref="ArgumentNullException">Если <paramref name="serviceProvider"/> равен null.</exception>
    /// <exception cref="InvalidOperationException">Если инициализация выполняется повторно.</exception>
    internal static void Initialize(IServiceProvider serviceProvider)
    {
        if (serviceProvider is null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (_services is not null && !ReferenceEquals(_services, serviceProvider))
            throw new InvalidOperationException("ServiceResolver уже инициализирован. Повторная инициализация запрещена.");

        _services = serviceProvider;
    }

    /// <summary>
    /// Освобождает корневой провайдер сервисов приложения, если он поддерживает <see cref="IDisposable"/>.
    /// </summary>
    internal static void Dispose()
    {
        if (_services is IDisposable disposable)
            disposable.Dispose();

        _services = null;
    }
}
