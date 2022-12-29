using Microsoft.Extensions.Logging;
using WPR.Data.Entities;
using WPR.Data.Repositories.Interfaces;

namespace WPR.Data.Repositories.Web;

public class NamedWebRepository<T> : WebRepository<T>, INamedRepository<T> where T : NamedEntity, new()
{
    public NamedWebRepository(HttpClient Client, ILogger<WebRepository<T>> Logger) : base(Client, Logger) { }

    public virtual async Task<bool> ExistNameAsync(string Name, CancellationToken Cancel = default) => await GetAsync<bool>($"{Address}/existname/{Name}", Cancel).ConfigureAwait(false);

    public virtual async Task<T> GetByNameAsync(string Name, CancellationToken Cancel = default) => await GetAsync<T>($"{Address}/getname/{Name}", Cancel).ConfigureAwait(false);
    public async Task<bool> UpdateNameAsync(int id, string newName, CancellationToken Cancel = default) =>
        await UpdatePropertyAsync(new T { Id = id, Name = newName }, item => item.Name, Cancel)
            .ConfigureAwait(false);
}