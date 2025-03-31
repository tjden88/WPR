using Microsoft.Extensions.DependencyInjection;

namespace WPR.Services.Implementations;

/// <summary>
/// Отложенная загрузка зависимостей.
/// Для предотвращения цмклических зависимостей
/// </summary>
class LazilyResolved<T>(IServiceProvider serviceProvider) : Lazy<T>(serviceProvider.GetRequiredService<T>) where T : notnull;