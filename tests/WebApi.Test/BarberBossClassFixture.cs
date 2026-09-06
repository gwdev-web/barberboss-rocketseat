using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebApi.Test;

[Collection(nameof(WebApiCollection))]
public abstract class BarberBossClassFixture
{
    protected readonly CustomWebApplicationFactory Factory;

    private readonly HttpClient _httpClient;

    protected BarberBossClassFixture(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        _httpClient = factory.CreateClient();
    }

    protected Task<HttpResponseMessage> DoGet(string route, string? token = null)
    {
        Authorize(token);

        return _httpClient.GetAsync(route);
    }

    protected Task<HttpResponseMessage> DoPost(string route, object body, string? token = null)
    {
        Authorize(token);

        return _httpClient.PostAsJsonAsync(route, body);
    }

    protected Task<HttpResponseMessage> DoPut(string route, object body, string? token = null)
    {
        Authorize(token);

        return _httpClient.PutAsJsonAsync(route, body);
    }

    protected Task<HttpResponseMessage> DoDelete(string route, string? token = null)
    {
        Authorize(token);

        return _httpClient.DeleteAsync(route);
    }

    private void Authorize(string? token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }
}

[CollectionDefinition(nameof(WebApiCollection))]
public class WebApiCollection : ICollectionFixture<CustomWebApplicationFactory> { }
