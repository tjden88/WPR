namespace WPR.Data.Entities.Interfaces;

/// <summary>
/// Сущность с пометкой удаления
/// </summary>
public interface IDeletedEntity : IEntity
{
    /// <summary>
    /// Удалена ли сущность
    /// </summary>
    bool IsDeleted { get; set; }
}