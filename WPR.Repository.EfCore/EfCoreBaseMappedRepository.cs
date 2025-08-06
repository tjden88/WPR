using Microsoft.EntityFrameworkCore;

namespace WPR.Repository.EfCore;

/// <summary>
/// Репозиторий для получения и изменения сущностей другого типа (с преобразованием в тип БД и обратно)
/// </summary>
/// <typeparam name="TSrc">Тип сущности БД</typeparam>
/// <typeparam name="TDest">Тип выходных и входных данных</typeparam>
public abstract class EfCoreBaseMappedRepository<TSrc, TDest>(DbContext Db) : EfCoreBaseRepository<TDest> where TDest : class where TSrc : class
{

    protected abstract IQueryable<TDest> MappedItems { get; }

    protected abstract TSrc MapToSource(TDest dest);

    protected virtual void OnItemAdded(TSrc item) { }
    protected virtual void OnItemUpdated(TSrc item) { }
    protected virtual void OnItemDeleted(TSrc item) { }


    #region Override

    protected sealed override IQueryable<TDest> Items => MappedItems.AsNoTracking();


    protected sealed override async Task<bool> AddItem(TDest item, CancellationToken cancellationToken)
    {
        var src = MapToSource(item);
        await Db.Set<TSrc>().AddAsync(src, cancellationToken).ConfigureAwait(false);
        OnItemAdded(src);
        return await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    protected sealed override async Task<bool> UpdateItem(TDest item, CancellationToken cancellationToken)
    {
        var src = MapToSource(item);
        Db.Set<TSrc>().Update(src);
        OnItemUpdated(src);
        return await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    }

    protected sealed override async Task<bool> DeleteItem(TDest item, CancellationToken cancellationToken)
    {
        var src = MapToSource(item);
        Db.Set<TSrc>().Remove(src);
        OnItemDeleted(src);
        return await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    protected override async Task<bool> DeleteItems(IQueryable<TDest> items, CancellationToken cancellationToken)
    {
        var result = await items
            .Select(i=> MapToSource(i))
            .ExecuteDeleteAsync(cancellationToken)
            .ConfigureAwait(false);

        return result > 0;
    }

    #endregion

    protected async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
    {
        var result = await Db.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;

        if (result)
            Db.ChangeTracker.Clear();

        return result;
    }
}