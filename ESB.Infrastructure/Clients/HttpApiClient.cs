using ESB.Configurations.Interfaces;
using ESB.Infrastructure.Services;

namespace ESB.Infrastructure.Clients;

public sealed class HttpApiClient(IHttpClientFactory httpClientFactory) : IApiClient<HttpRequestMessage, HttpResponseMessage>
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    
    public void Initialize()
    {
        Console.WriteLine("HttpApiClient initialized");
    }

    public async Task<HttpResponseMessage> SendMessageAsync(HttpRequestMessage requestMessage)
    {
        return await _httpClient.SendAsync(requestMessage);
    }
}