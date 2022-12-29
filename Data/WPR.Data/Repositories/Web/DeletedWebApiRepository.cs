using Microsoft.Extensions.Logging;
using WPR.Data.Entities;
using WPR.Data.Entities.Interfaces;
using WPR.Data.Repositories.Interfaces;

namespace WPR.Data.Repositories.Web;

/// <summary>
/// Репозиторий Web-Api для удалённых сущностей
/// </summary>
/// <typeparam name="T"></typeparam>
public class DeletedWebApiRepository<T> : WebApiRepository<T>, IDeletedRepository<T> where T : Entity, IDeletedEntity, new()
{
    public DeletedWebApiRepository(HttpClient Client, ILogger<WebApiRepository<T>> Logger) : base(Client, $"api/Deleted{typeof(T).Name}", Logger) { }


    public virtual async Task<T?> RestoreAsync(int id, CancellationToken Cancel = default) =>
        await GetAsync<T>($"{Address}/restore/{id}", Cancel).ConfigureAwait(false);
}