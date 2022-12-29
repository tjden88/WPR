using Microsoft.Extensions.Logging;
using WPR.Data.Entities;
using WPR.Data.Entities.Interfaces;
using WPR.Data.Repositories.Interfaces;

namespace WPR.Data.Repositories.Web;

public class DeletedWebRepository<T> : WebRepository<T>, IDeletedRepository<T> where T : Entity, IDeletedEntity, new()
{
    public DeletedWebRepository(HttpClient Client, ILogger<WebRepository<T>> Logger) : base(Client, $"api/Deleted{typeof(T).Name}", Logger) { }


    public virtual async Task<T> RestoreAsync(int id, CancellationToken Cancel = default) => await GetAsync<T>($"{Address}/restore/{id}", Cancel).ConfigureAwait(false);
}