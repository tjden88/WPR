using Microsoft.EntityFrameworkCore;
using WPR.Data.Base.Entities;
using WPR.Data.Repositories.Interfaces;

namespace WPR.Data.Repositories.EntityFramework;

/// <summary>
/// Репозиторий именованных сущностей БД
/// </summary>
/// <typeparam name="T">Именованная сущность</typeparam>
/// <typeparam name="TKey">Тип идентификатора сущности</typeparam>
public class DbNamedRepository<T, TKey> : DbRepository<T, TKey>, INamedRepository<T, TKey> where T : NamedEntity<TKey>, new() where TKey : notnull
{
    public DbNamedRepository(DbContext Db) : base(Db) { }

    public virtual async Task<bool> ExistNameAsync(string Name, CancellationToken Cancel = default) =>
        await Items.AnyAsync(item => item.Name == Name, Cancel).ConfigureAwait(false);

    public virtual async Task<T?> GetByNameAsync(string Name, CancellationToken Cancel = default) =>
        await Items.FirstOrDefaultAsync(i => i.Name == Name, Cancel).ConfigureAwait(false);

    public async Task<bool> UpdateNameAsync(TKey id, string newName, CancellationToken Cancel = default) =>
        await UpdatePropertyAsync(new T { Id = id, Name = newName }, item => item.Name, Cancel)
            .ConfigureAwait(false);
}