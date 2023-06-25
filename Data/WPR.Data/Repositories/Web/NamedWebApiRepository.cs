using Microsoft.Extensions.Logging;
using WPR.Data.Base.Entities.Interfaces;
using WPR.Data.Repositories.Interfaces;

namespace WPR.Data.Repositories.Web;

/// <summary>
/// Репозиторий Web-Api именованной сущности
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TKey">Тип идентификатора сущности</typeparam>
public class NamedWebApiRepository<T, TKey> : WebApiRepository<T, TKey>, INamedRepository<T, TKey> where T : INamedEntity<TKey>, new() where TKey : notnull
{
    public NamedWebApiRepository(HttpClient Client, ILogger<WebApiRepository<T, TKey>> Logger) : base(Client, Logger) { }

    public virtual async Task<bool> ExistNameAsync(string Name, CancellationToken Cancel = default) =>
        await GetAsync<bool>($"{Address}/existname/{Name}", Cancel).ConfigureAwait(false);


    public virtual async Task<T?> GetByNameAsync(string Name, CancellationToken Cancel = default) =>
        await GetAsync<T>($"{Address}/getname/{Name}", Cancel).ConfigureAwait(false);


    public async Task<bool> UpdateNameAsync(TKey id, string newName, CancellationToken Cancel = default) =>
        await UpdatePropertyAsync(new T { Id = id, Name = newName }, item => item.Name, Cancel)
            .ConfigureAwait(false);
}