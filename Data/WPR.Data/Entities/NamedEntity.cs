using WPR.Data.Entities.Interfaces;

namespace WPR.Data.Entities;

public abstract class NamedEntity<TKey> : Entity<TKey>, INamedEntity<TKey> where TKey : notnull
{
    public string Name { get; set; } = null!;
}