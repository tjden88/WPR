using Microsoft.EntityFrameworkCore;
using WPR.Data.Entities;
using WPR.Data.Entities.Interfaces;
using WPR.Data.Repositories.Interfaces;

namespace WPR.Data.Repositories.EntityFramework;

/// <summary>
/// Репозиторий удалённых сущностей БД
/// </summary>
/// <typeparam name="T">IDeletedEntity</typeparam>
public class DbDeletedRepository<T> : DbRepository<T>, IDeletedRepository<T> where T : Entity, IDeletedEntity, new()
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


    public async Task<T?> RestoreAsync(int id, CancellationToken Cancel = default)
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