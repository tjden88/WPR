using System.Linq.Expressions;

namespace WPR.Repository.Abstractions;

/// <summary>
/// Репозиторий для выборки связанных данных из двух различных источников,
/// объединённых по определённому условию. 
/// </summary>
/// <typeparam name="T">"Левая" сущность</typeparam>
/// <typeparam name="T1">"Правая" сущность</typeparam>
public interface ICompositeRepository<T, T1> 
    where T : class 
    where T1 : class
{
    /// <summary>
    /// Получить проекцию на основе пар элементов (<typeparamref name="T"/>, <typeparamref name="T1"/>).
    /// </summary>
    /// <typeparam name="TResult">Тип результата выборки</typeparam>
    /// <param name="selector">Выражение проекции из пары (T, T1) в TResult</param>
    /// <param name="predicate">Фильтр для отбора пар</param>
    /// <param name="orderBy">Функция сортировки по типу результата</param>
    /// <param name="includeUnmatched">Получить все "Левые" значения, вне зависимости от включения "Правых"</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список элементов типа TResult</returns>
    Task<List<TResult>> GetAsync<TResult>(
        Expression<Func<T, T1, TResult>> selector,
        Expression<Func<T, T1, bool>>? predicate = null,
        Func<IQueryable<TResult>, IOrderedQueryable<TResult>>? orderBy = null,
        bool includeUnmatched = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить список "левых" сущностей <typeparamref name="T"/>, участвующих в связке.
    /// </summary>
    /// <param name="predicate">Фильтр для отбора пар</param>
    /// <param name="orderBy">Функция сортировки</param>
    /// <param name="includeUnmatched">Получить все "Левые" значения, вне зависимости от включения "Правых"</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список уникальных сущностей <typeparamref name="T"/></returns>
    Task<List<T>> GetLeftsAsync(
        Expression<Func<T, T1, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool includeUnmatched = false, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить список "правых" сущностей <typeparamref name="T1"/>, участвующих в связке.
    /// </summary>
    /// <param name="predicate">Фильтр для отбора пар</param>
    /// <param name="orderBy">Функция сортировки</param>
    /// <param name="includeUnmatched">Получить все "Левые" значения, вне зависимости от включения "Правых"</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список уникальных сущностей <typeparamref name="T1"/></returns>
    Task<List<T1>> GetRightsAsync(
        Expression<Func<T, T1, bool>>? predicate = null,
        Func<IQueryable<T1>, IOrderedQueryable<T1>>? orderBy = null,
        bool includeUnmatched = false, 
        CancellationToken cancellationToken = default);
}