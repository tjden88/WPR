namespace WPR.Data.Base.Entities.Interfaces;

/// <summary> Интерфейс сущности </summary>
public interface IEntity<TKey> where TKey: notnull
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    TKey Id { get; set; }
}