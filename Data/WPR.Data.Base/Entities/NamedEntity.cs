using WPR.Data.Base.Entities.Interfaces;

namespace WPR.Data.Base.Entities;

public abstract class NamedEntity<TKey> : Entity<TKey>, INamedEntity<TKey> where TKey : notnull
{
    public string Name { get; set; } = null!;
}