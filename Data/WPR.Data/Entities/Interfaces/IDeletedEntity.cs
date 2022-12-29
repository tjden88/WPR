namespace WPR.Data.Entities.Interfaces;

/// <summary>
/// Сущность с пометкой удаления
/// </summary>
public interface IDeletedEntity<TKey> : IEntity<TKey> where TKey : notnull
{
    /// <summary>
    /// Удалена ли сущность
    /// </summary>
    bool IsDeleted { get; set; }
}