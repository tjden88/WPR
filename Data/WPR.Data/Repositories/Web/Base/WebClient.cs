using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace WPR.Data.Repositories.Web.Base;

public abstract class WebClient
{
    protected readonly ILogger Logger;

    /// <summary> Http-клиент </summary>
    protected HttpClient Http { get; }

    /// <summary> Базовый адрес Http-клиента </summary>
    protected string Address { get; }

    protected WebClient(HttpClient Client, string Address, ILogger Logger)
    {
        this.Logger = Logger;
        Http = Client;
        this.Address = Address;

    }

    /// <summary> Проверить доступность сервера. По умолчанию - на базовый адрес клиента </summary>
    public bool CheckConnection(string OptionalUrl = "") => CheckConnectionAsync(OptionalUrl).Result;


    /// <summary> Проверить доступность сервера. По умолчанию - на базовый адрес клиента </summary>
    public async Task<bool> CheckConnectionAsync(string OptionalUrl = "")
    {
        try
        {
            await Http.SendAsync(new HttpRequestMessage(HttpMethod.Head, $"{Address}{OptionalUrl}"));
            return true;
        }
        catch (HttpRequestException e)
        {
            Logger.LogError(e, "Ошибка HEAD запроса : {0}", e.Message);
            return false;
        }
    }

    /// <summary> Отправить GET запрос </summary>
    protected T Get<T>(string url) => GetAsync<T>(url).Result;

    /// <summary> Отправить GET запрос </summary>
    protected async Task<T> GetAsync<T>(string url, CancellationToken Cancel = default)
    {
        try
        {
            var response = await Http.GetAsync(url, Cancel).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode) return default;
            return await response
                .Content
                .ReadFromJsonAsync<T>(cancellationToken: Cancel)
                .ConfigureAwait(false);
        }
        catch (HttpRequestException e)
        {
            Logger.LogError(e, "Ошибка GET запроса : {0}", e.Message);
            return default;
        }
    }


    /// <summary> Отправить POST запрос </summary>
    protected HttpResponseMessage Post<T>(string url, T value) => PostAsync(url, value).Result;

    /// <summary> Отправить POST запрос </summary>
    protected async Task<HttpResponseMessage> PostAsync<T>(string url, T value, CancellationToken Cancel = default)
    {
        try
        {
            var response = await Http.PostAsJsonAsync(url, value, Cancel).ConfigureAwait(false);
            return response;
        }
        catch (HttpRequestException e)
        {
            Logger.LogError(e, "Ошибка POST запроса : {0}", e.Message);
            return default;
        }
    }


    /// <summary> Отправить PUT запрос </summary>
    protected HttpResponseMessage Put<T>(string url, T value) => PutAsync(url, value).Result;

    /// <summary> Отправить PUT запрос </summary>
    protected async Task<HttpResponseMessage> PutAsync<T>(string url, T value, CancellationToken Cancel = default)
    {
        try
        {
            var response = await Http.PutAsJsonAsync(url, value, Cancel).ConfigureAwait(false);
            return response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Logger.LogError(e, "Ошибка PUT запроса : {0}", e.Message);
            return default;
        }
    }


    /// <summary> Отправить DELETE запрос </summary>
    protected HttpResponseMessage Delete(string url) => DeleteAsync(url).Result;

    /// <summary> Отправить DELETE запрос </summary>
    protected async Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken Cancel = default)
    {
        try
        {
            var response = await Http.DeleteAsync(url, Cancel).ConfigureAwait(false);
            return response;
        }
        catch (HttpRequestException e)
        {
            Logger.LogError(e, "Ошибка DELETE запроса : {0}", e.Message);
            return default;
        }
    }
}