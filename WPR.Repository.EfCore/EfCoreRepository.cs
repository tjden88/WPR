using Microsoft.EntityFrameworkCore;

namespace WPR.Repository.EfCore;

/// <summary>
/// Реализация репозитория для сущностей модели базы данных Entity Framework Core
/// </summary>
/// <typeparam name="T">Сущность БД</typeparam>
/// <typeparam name="TContext">Контекст БД</typeparam>
public class EfCoreRepository<T, TContext>(TContext Db) : EfCoreBaseRepository<T> where TContext : DbContext where T : class
{

    protected override IQueryable<T> Items => Db.Set<T>();


    protected async Task<bool> SaveChangesAsync(CancellationToken cancellationToken) => await Db.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;


    protected override async Task<bool> AddItem(T item, CancellationToken cancellationToken )
    {
        await Db.AddAsync(item, cancellationToken).ConfigureAwait(false);
        return await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    protected override async Task<bool> UpdateItem(T item, CancellationToken cancellationToken)
    {
        Db.Update(item);
        return await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    protected override async Task<bool> DeleteItem(T item, CancellationToken cancellationToken)
    {
        var result = await Items.Where(i => i.Equals(item))
            .ExecuteDeleteAsync(cancellationToken)
            .ConfigureAwait(false);

        return result > 0;
    }

    protected override async Task<bool> DeleteItems(IQueryable<T> items, CancellationToken cancellationToken)
    {
        var result = await items
            .ExecuteDeleteAsync(cancellationToken)
            .ConfigureAwait(false);

        return result > 0;
    }
}