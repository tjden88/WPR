using System.Linq.Expressions;

namespace WPR.Repository.Abstractions;

/// <summary>
/// CRUD епозиторий сущностей
/// </summary>
/// <typeparam name="TEntity">Класс сущности</typeparam>
public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
{

    #region Add

    /// <summary>
    /// Добавить сущность в репозиторий
    /// </summary>
    /// <param name="item">Добавляемая сущность</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<bool> AddAsync(TEntity item, CancellationToken cancellationToken = default);

    #endregion

    #region Update

    /// <summary>
    /// Обновить сущность в репозитории
    /// </summary>
    /// <param name="item">Изменяемая сущность</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<bool> UpdateAsync(TEntity item, CancellationToken cancellationToken = default);

    #endregion

    #region Delete

    /// <summary>
    /// Удалить сущность из репозитория
    /// </summary>
    /// <param name="item">Удаляемая сущность</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<bool> DeleteAsync(TEntity item, CancellationToken cancellationToken = default);


    /// <summary>
    /// Удалить сущности из репозитория по предикату
    /// </summary>
    /// <param name="predicate">Выборка сущностей. Будут удалены все подходящие сущности</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>False, если не найдено ни одной подходящей сущности</returns>
    Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    #endregion

}
