using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WPR.Repository.Abstractions;

namespace WPR.Repository.EfCore;

/// <summary>
/// Базовый абстрактный репозиторий сущностей БД
/// </summary>
public abstract class EfCoreBaseRepository<T> : EfCoreBaseReadOnlyRepository<T>, IRepository<T> where T : class
{

    protected abstract Task<bool> AddItem(T item, CancellationToken cancellationToken);
    protected abstract Task<bool> UpdateItem(T item, CancellationToken cancellationToken);
    protected abstract Task<bool> DeleteItem(T item, CancellationToken cancellationToken);
    protected abstract Task<bool> DeleteItems(IQueryable<T> items, CancellationToken cancellationToken );



    #region IRepository Methods

    public async Task<bool> AddAsync(T item, CancellationToken cancellationToken = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return await AddItem(item, cancellationToken).ConfigureAwait(false);

    }

    public async Task<bool> UpdateAsync(T item, CancellationToken cancellationToken = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return await UpdateItem(item, cancellationToken).ConfigureAwait(false);

    }
    public async Task<bool> DeleteAsync(T item, CancellationToken cancellationToken = default)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return await DeleteItem(item, cancellationToken).ConfigureAwait(false);

    }

    public async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));

        var toDelete = Items.Where(predicate);

        var queryCount = await toDelete.CountAsync(cancellationToken).ConfigureAwait(false);

        if (queryCount == 0) return false;
        return await DeleteItems(toDelete, cancellationToken).ConfigureAwait(false);
    }

    #endregion
}