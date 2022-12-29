using Microsoft.EntityFrameworkCore;
using WPR.Data.Base.Entities;
using WPR.Data.Base.Entities.Interfaces;
using WPR.Data.Repositories.Interfaces;

namespace WPR.Data.Repositories.EntityFramework;

/// <summary>
/// Репозиторий удалённых сущностей БД
/// </summary>
/// <typeparam name="T">IDeletedEntity</typeparam>
/// <typeparam name="TKey">Тип идентификатора сущности</typeparam>
public class DbDeletedRepository<T, TKey> : DbRepository<T, TKey>, IDeletedRepository<T, TKey> where T : Entity<TKey>, IDeletedEntity<TKey>, new() where TKey : notnull
{
    public DbDeletedRepository(DbContext Db) : base(Db) { }


    public override IQueryable<T> Items => Set.Where(item => item.IsDeleted);


    public override Task<T?> AddAsync(T item, CancellationToken Cancel = default)
    {
        item.IsDeleted = true;
        return base.AddAsync(item, Cancel);
    }


    public override Task<int> AddRangeAsync(IEnumerable<T> items, CancellationToken Cancel = default)
    {
        var deletedEntities = items as T[] ?? items.ToArray();
        foreach (var deletedEntity in deletedEntities)
            deletedEntity.IsDeleted = true;

        return base.AddRangeAsync(deletedEntities, Cancel);
    }


    protected override void MarkDeletedOrDelete(IEnumerable<T> items) =>
        Set.RemoveRange(items);


    public async Task<T?> RestoreAsync(TKey id, CancellationToken Cancel = default)
    {
        var entity = await GetByIdAsync(id, Cancel).ConfigureAwait(false);

        if (entity is null)
            return null;

        entity.IsDeleted = false;
        var restored = await UpdatePropertyAsync(entity, item => item.IsDeleted, Cancel);

        return restored
            ? entity
            : null;
    }
}