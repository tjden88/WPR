namespace WPR.Data.Entities.Interfaces;

/// <summary>
/// Именованная сущность
/// </summary>
public interface INamedEntity : IEntity
{
    /// <summary> Имя сущности. Обязательное свойство </summary>
    string Name { get; set; }
}