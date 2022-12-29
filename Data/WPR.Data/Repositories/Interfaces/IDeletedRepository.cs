using WPR.Data.Entities.Interfaces;

namespace WPR.Data.Repositories.Interfaces;

/// <summary>
/// Репозиторий сущностей, помеченных как удалённые.
/// Методы получения возвращают коллекции и единичные элементы удалённых сущностей.
/// Методы добавления устанавливают свойство IsDeleted = true.
/// Методы обновления работают как обычно.
/// Методы удаления - удаляют сущность окончательно
/// </summary>
/// <typeparam name="TDeletedEntity"></typeparam>
public interface IDeletedRepository<TDeletedEntity> : IRepository<TDeletedEntity> where TDeletedEntity : IEntity, IDeletedEntity
{

    /// <summary>
    /// Восстановить удалённую сущность
    /// </summary>
    /// <returns>null, если не удалось</returns>
    Task<TDeletedEntity> RestoreAsync(int id, CancellationToken Cancel = default);
}