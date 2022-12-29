using WPR.Data.Entities.Interfaces;

namespace WPR.Data.Entities;

public class NamedEntity : Entity, INamedEntity
{
    public string Name { get; set; } = null!;
}