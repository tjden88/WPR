namespace WPR.Data.Base.Entities.Interfaces;

/// <summary>
/// Именованная сущность
/// </summary>
public interface INamedEntity<TKey> : IEntity<TKey> where TKey : notnull
{
    /// <summary> Имя сущности. Обязательное свойство </summary>
    string Name { get; set; }
}