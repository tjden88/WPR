using Microsoft.Extensions.Logging;
using WPR.Data.Base.Entities.Interfaces;
using WPR.Data.Repositories.Interfaces;

namespace WPR.Data.Repositories.Web;

/// <summary>
/// Репозиторий Web-Api для удалённых сущностей
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TKey">Тип идентификатора сущности</typeparam>
public class DeletedWebApiRepository<T, TKey> : WebApiRepository<T, TKey>, IDeletedRepository<T, TKey> where T : IDeletedEntity<TKey>, new() where TKey : notnull
{
    public DeletedWebApiRepository(HttpClient Client, ILogger<WebApiRepository<T, TKey>> Logger) : base(Client, $"api/Deleted{typeof(T).Name}", Logger) { }


    public virtual async Task<T?> RestoreAsync(TKey id, CancellationToken Cancel = default) =>
        await GetAsync<T>($"{Address}/restore/{id}", Cancel).ConfigureAwait(false);
}