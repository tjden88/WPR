using WPR.Data.Entities.Interfaces;

namespace WPR.Data.Entities;

public abstract class DeletedEntity<TKey> : Entity<TKey>, IDeletedEntity<TKey> where TKey : notnull
{
    public bool IsDeleted { get; set; }
}