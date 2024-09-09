using ESB.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESB.Infrastructure.Clients;

public sealed class HttpApiClient(IHttpClientFactory httpClientFactory, ILogger<IAdapterDi> logger) : IApiClient<HttpRequestMessage, HttpResponseMessage>
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    
    public void Initialize()
    {
        // await AuthorizeClient(_httpClient);
        logger.LogInformation("Successful HttpApiClient Initialization");
    }

    // private async Task AuthorizeClient(HttpClient client)
    // {
    //     await authorizer.AuthenticateAsync(client);
    // }

    public async Task<HttpResponseMessage> SendMessageAsync(HttpRequestMessage requestMessage)
    {
        var response = await _httpClient.SendAsync(requestMessage);
        return response;
    }
}