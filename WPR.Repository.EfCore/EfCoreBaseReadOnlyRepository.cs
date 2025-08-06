using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WPR.Repository.Abstractions;

namespace WPR.Repository.EfCore;


/// <summary>
/// Базовый репозиторий сущностей БД только для чтения
/// </summary>
public abstract class EfCoreBaseReadOnlyRepository<T> : IReadOnlyRepository<T> where T : class
{

    /// <summary> Коллекция для работы с репозиторием </summary>
    protected abstract IQueryable<T> Items { get; }


    #region Get

    public async Task<IList<T>> GetAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        var query = predicate is null
            ? Items
            : Items.Where(predicate);

        return orderBy is not null
            ? await orderBy(query).ToArrayAsync(cancellationToken).ConfigureAwait(false)
            : await query.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IPage<T>> GetPageAsync(int pageSize, int pageIndex = 1, Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        if (pageIndex < 1)
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Номер страницы должен быть не меньше единицы!");

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Размер страницы должен быть не меньше единицы!");

        var query = predicate is null
            ? Items
            : Items.Where(predicate);

        var totalCount = await query
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

        if (pageIndex > 1)
            query = query.Skip((pageIndex - 1) * pageSize);

        query = query.Take(pageSize);

        var items = orderBy is null
            ? await query.ToListAsync(cancellationToken).ConfigureAwait(false)
            : await orderBy(query).ToListAsync(cancellationToken).ConfigureAwait(false);

        return new Page<T>(items, totalCount, pageIndex, pageSize);

    }

    public virtual async Task<T?> GetOneAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) =>
        predicate is null
            ? await Items.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)
            : await Items.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);


    #endregion

    #region Exist, Count

    public virtual async Task<bool> ExistAsync(Expression<Func<T, bool>>? selector = null, CancellationToken cancellationToken = default) =>
        selector is null
            ? await Items.AnyAsync(cancellationToken).ConfigureAwait(false)
            : await Items.AnyAsync(selector, cancellationToken).ConfigureAwait(false);


    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) =>
        predicate is null
            ? await Items.CountAsync(cancellationToken).ConfigureAwait(false)
            : await Items.CountAsync(predicate, cancellationToken).ConfigureAwait(false);



    #endregion

}