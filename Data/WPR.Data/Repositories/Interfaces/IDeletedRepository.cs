using WPR.Data.Base.Entities.Interfaces;

namespace WPR.Data.Repositories.Interfaces;

/// <summary>
/// Репозиторий сущностей, помеченных как удалённые.
/// Методы получения возвращают коллекции и единичные элементы удалённых сущностей.
/// Методы добавления устанавливают свойство IsDeleted = true.
/// Методы обновления работают как обычно.
/// Методы удаления - удаляют сущность окончательно
/// </summary>
/// <typeparam name="TDeletedEntity">Тип, реализующий IDeletedEntity</typeparam>
/// <typeparam name="TKey">Тип идентификатора сущности</typeparam>
public interface IDeletedRepository<TDeletedEntity, in TKey> : IRepository<TDeletedEntity, TKey> where TDeletedEntity : IEntity<TKey>, IDeletedEntity<TKey> where TKey : notnull
{

    /// <summary>
    /// Восстановить удалённую сущность
    /// </summary>
    /// <returns>null, если не удалось</returns>
    Task<TDeletedEntity?> RestoreAsync(TKey id, CancellationToken Cancel = default);
}