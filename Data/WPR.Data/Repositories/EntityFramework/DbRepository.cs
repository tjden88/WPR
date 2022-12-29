using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WPR.Data.Entities;
using WPR.Data.Entities.Interfaces;
using WPR.Data.Paging;
using WPR.Data.Paging.Interfaces;
using WPR.Data.Repositories.Interfaces;

namespace WPR.Data.Repositories.EntityFramework;

/// <summary>
/// Репозиторий сущностей БД
/// </summary>
/// <typeparam name="T">Сущность БД</typeparam>
/// <typeparam name="TKey">Тип идентификатора сущности</typeparam>
public class DbRepository<T, TKey> : IRepository<T, TKey> where T : Entity<TKey>, new() where TKey : notnull
{
    private readonly DbContext _Db; // Контекст БД

    private static bool IsDeletedEntity => typeof(IDeletedEntity<TKey>).IsAssignableFrom(typeof(T));


    /// <summary> Набор данных БД </summary>
    protected DbSet<T> Set => _Db.Set<T>();

    public DbRepository(DbContext Db)
    {
        _Db = Db;
    }

    #region IRepository

    public virtual IQueryable<T> Items
    {
        get
        {
            IQueryable<T> itemsQuery = Set;

            if (IsDeletedEntity)
                itemsQuery = itemsQuery.Where(item => !((IDeletedEntity<TKey>)item).IsDeleted);

            return itemsQuery;
        }
    }


    public virtual IQueryable<T> Get(Expression<Func<T, bool>> Filter) => Items.Where(Filter);

    public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> Filter, CancellationToken Cancel = default) => await Get(Filter).ToArrayAsync(Cancel);


    public virtual async Task<IPage<T>> GetPage(int PageIndex, int PageSize, Expression<Func<T, object>>? OrderExpression = null, bool Ascending = true, CancellationToken Cancel = default) =>
        await GetPage(new PageFilter<T>
        {
            PageIndex = PageIndex,
            PageSize = PageSize,
            OrderBy = OrderExpression is null
                ? null
                : new PageOrderInfo<T>(OrderExpression, Ascending),
        }, Cancel)
            .ConfigureAwait(false);


    public virtual async Task<IPage<T>> GetPage(IPageFilter<T> Filter, CancellationToken Cancel = default)
    {
        var pageIndex = Filter.PageIndex;
        var pageSize = Filter.PageSize;

        if (pageIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Номер страницы не может быть отрицательным");

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Размер страницы должен быть больше нуля");


        var query = Filter.Filter is null
            ? Items
            : Items.Where(Filter.Filter);

        var count = await query.CountAsync(Cancel).ConfigureAwait(false);

        if (Filter.OrderBy is { } orderBy)
            query = orderBy.Ascending
                ? query.OrderBy(orderBy.OrderExpression)
                : query.OrderByDescending(orderBy.OrderExpression);
        else
            query = query.OrderBy(item => item.Id);


        if (Filter.ThenOrderBy?.Any() == true)
            foreach (var thenOrder in Filter.ThenOrderBy)
                query = thenOrder.Ascending
                    ? ((IOrderedQueryable<T>)query).ThenBy(thenOrder.OrderExpression)
                    : ((IOrderedQueryable<T>)query).ThenByDescending(thenOrder.OrderExpression);


        if (pageIndex > 0)
            query = query.Skip(pageIndex * pageSize);

        query = query.Take(pageSize);

        return new Page<T>(await query.ToArrayAsync(Cancel).ConfigureAwait(false), count, pageIndex, pageSize);
    }


    public virtual async Task<int> CountAsync(CancellationToken Cancel = default) => await Items.CountAsync(Cancel).ConfigureAwait(false);


    public virtual async Task<bool> ExistAsync(TKey id, CancellationToken Cancel = default) =>
        await Items.AnyAsync(item => Equals(id, item.Id), Cancel).ConfigureAwait(false);


    public virtual async Task<T?> GetByIdAsync(TKey id, CancellationToken Cancel = default) =>
        await Items.FirstOrDefaultAsync(item => Equals(id, item.Id), Cancel).ConfigureAwait(false);


    public virtual async Task<T?> AddAsync(T item, CancellationToken Cancel = default)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        await _Db.AddAsync(item, Cancel).ConfigureAwait(false);

        var result = await _Db.SaveChangesAsync(Cancel).ConfigureAwait(false) > 0;

        return result
            ? item
            : null;
    }


    public virtual async Task<int> AddRangeAsync(IEnumerable<T> items, CancellationToken Cancel = default)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        await _Db.AddRangeAsync(items, Cancel).ConfigureAwait(false);

        return await _Db.SaveChangesAsync(Cancel).ConfigureAwait(false);
    }


    public virtual async Task<bool> UpdateAsync(T item, CancellationToken Cancel = default)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        _Db.Update(item);

        var changesCount = await _Db.SaveChangesAsync(Cancel).ConfigureAwait(false);
        var result = changesCount > 0;

        return result;
    }

    public virtual async Task<int> UpdateRangeAsync(IEnumerable<T> items, CancellationToken Cancel = default)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        _Db.UpdateRange(items);

        return await _Db.SaveChangesAsync(Cancel).ConfigureAwait(false);
    }


    public virtual async Task<bool> UpdatePropertyAsync(T item, Expression<Func<T, object>> property, CancellationToken Cancel = default) =>
        await UpdatePropertiesAsync(item, new[] { property }, Cancel);


    public virtual async Task<bool> UpdatePropertiesAsync(T item, IEnumerable<Expression<Func<T, object>>> properties, CancellationToken Cancel = default)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        if (!await ExistAsync(item.Id, Cancel))
            return false;
        _Db.ChangeTracker.AutoDetectChangesEnabled = false;
        _Db.Attach(item);
        _Db.Entry(item).State = EntityState.Unchanged;
        foreach (var property in properties)
            _Db.Entry(item).Property(property).IsModified = true;

        _Db.ChangeTracker.AutoDetectChangesEnabled = true;
        return await _Db.SaveChangesAsync(Cancel).ConfigureAwait(false) > 0;
    }


    public virtual async Task<bool> DeleteAsync(TKey id, CancellationToken Cancel = default)
    {
        if (!await ExistAsync(id, Cancel))
            return false;

        var item = Set.Local.FirstOrDefault(i => Equals(id, i.Id)) ?? new T { Id = id };

        _Db.Attach(item);

        MarkDeletedOrDelete(new[] { item });

        return await _Db.SaveChangesAsync(Cancel).ConfigureAwait(false) > 0;
    }


    public virtual async Task<int> DeleteRangeAsync(IEnumerable<TKey> ids, CancellationToken Cancel = default)
    {
        var existing = await Items.Select(item => item.Id)
            .Where(id => ids.Contains(id))
            .ToArrayAsync(Cancel)
            .ConfigureAwait(false);

        var itemsToDelete = existing
            .Select(id => Set.Local.FirstOrDefault(i => Equals(id, i.Id)) ?? new T { Id = id });

        MarkDeletedOrDelete(itemsToDelete);

        return await _Db.SaveChangesAsync(Cancel).ConfigureAwait(false);
    }

    #endregion


    /// <summary> Пометить на удаление или удалить окончательно </summary>
    protected virtual void MarkDeletedOrDelete(IEnumerable<T> items)
    {
        if (IsDeletedEntity)
        {
            foreach (var item in items)
            {
                var deletedEntity = (IDeletedEntity<TKey>)item;
                _Db.Entry(deletedEntity).State = EntityState.Unchanged;
                deletedEntity.IsDeleted = true;
                _Db.Entry(deletedEntity)
                    .Property(delItem => delItem.IsDeleted).IsModified = true;
            }
        }
        else
            _Db.RemoveRange(items);
    }
}