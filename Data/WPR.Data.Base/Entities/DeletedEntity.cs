using WPR.Data.Base.Entities.Interfaces;

namespace WPR.Data.Base.Entities;

public abstract class DeletedEntity<TKey> : Entity<TKey>, IDeletedEntity<TKey> where TKey : notnull
{
    public bool IsDeleted { get; set; }
}