using Microsoft.Extensions.DependencyInjection;

namespace WPR.UiServices.UI;

/// <summary>
/// Отложенная загрузка зависимостей.
/// Для предотвращения цмклических зависимостей
/// </summary>
class LazilyResolved<T> : Lazy<T> where T : notnull
{
    public LazilyResolved(IServiceProvider serviceProvider)
        : base(serviceProvider.GetRequiredService<T>)
    {
    }
}