namespace WPR.Repository.Abstractions;

/// <summary>
/// Страница ограниченной выборки данных
/// </summary>
public interface IPage<out T> : IReadOnlyList<T>
{
    /// <summary>
    /// Номер страницы (начиная с единицы)
    /// </summary>
    int PageIndex { get; }

    /// <summary>
    /// Размер страницы
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Всего страниц
    /// </summary>
    int TotalPagesCount { get; }

    /// <summary>
    /// Есть предыдущая страница
    /// </summary>
    bool HasPreviousPage { get; }

    /// <summary>
    /// Есть следующая страница
    /// </summary>
    bool HasNextPage { get; }

    /// <summary>
    /// Всего больше одной страницы
    /// </summary>
    bool HasManyPages { get; }

    /// <summary>
    /// Общее количество сущностей в выборке
    /// </summary>
    int TotalItemsCount { get; }
}