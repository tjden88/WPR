using WPR.Data.Base.Entities.Interfaces;

namespace WPR.Data.Base.Entities.EntityFramework;

public abstract class NamedDbEntity : DbEntity, INamedEntity<int>
{
    public string Name { get; set; } = null!;
}