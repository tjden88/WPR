namespace WPR.Data.Entities.Interfaces;

public interface INamedEntity : IEntity
{
    string Name { get; set; }
}