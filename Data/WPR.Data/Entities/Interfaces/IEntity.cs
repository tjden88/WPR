namespace WPR.Data.Entities.Interfaces;

/// <summary> Интерфейс сущности </summary>
public interface IEntity
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    int Id { get; set; }
}