using System.Linq.Expressions;

namespace WPR.Repository.Abstractions;

/// <summary>
/// Репозиторий чтения сущностей
/// </summary>
/// <typeparam name="TEntity">Класс сущности</typeparam>
public interface IReadOnlyRepository<TEntity> where TEntity : class
{
    #region Get

    /// <summary>
    /// Получить элементы из репозитория с возможностью выборки и сортировки
    /// </summary>
    /// <param name="predicate">Функция для отбора элементов</param>
    /// <param name="orderBy">Функция сортировки выбранных элементов</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пустое перечисление, если ничего не найдено по выбранным параметрам</returns>
    Task<IList<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Постраничная выборка элементов с сортировкой данных
    /// </summary>
    /// <param name="pageIndex">Номер страницы (1 или больше)</param>
    /// <param name="pageSize">Размер страницы</param>
    /// <param name="predicate">Функция для отбора элементов</param>
    /// <param name="orderBy">Функция сортировки выбранных элементов</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<IPage<TEntity>> GetPageAsync(int pageSize, int pageIndex = 1, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, CancellationToken cancellationToken = default);


    /// <summary>
    /// Получить единственную сущность по предикату
    /// </summary>
    /// <param name="predicate">Функция для отбора элементов. Если null - будет возвращён первый попавшийся</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>null, если ничего не нашлось, либо первая подходящая</returns>
    Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);


    #endregion

    #region Exists

    /// <summary>
    /// Проверить существование элемента по предикату. Если предикат не задан - проверяет наличие любой сущности в репозитории
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <param name="selector">Функция для поиска элемента</param>
    /// <returns></returns>
    Task<bool> ExistAsync(Expression<Func<TEntity, bool>>? selector = null, CancellationToken cancellationToken = default);

    #endregion

    #region Count

    /// <summary> Получить количество сущностей. Если предикат не задан - получить количество всех сущностей</summary>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    #endregion
}