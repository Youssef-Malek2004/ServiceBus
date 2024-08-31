using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.Infrastructure.Factories;
using Microsoft.AspNetCore.Http;

namespace ESB.Infrastructure.Adapters;

public class HttpAdapter(EsbRoute esbRoute, ClientFactory clientFactory, IHttpClientFactory httpClientFactory) : IAdapterDi, IAdapter<HttpRequestMessage, HttpResponseMessage, HttpContext>
{
     private readonly dynamic _apiClient = clientFactory.CreateClient(esbRoute.SendLocation, httpClientFactory);
     
    public void Initialize()
    {
        _apiClient.Initialize();
        Console.WriteLine("HttpAdapter initialized with API client.");
    }

    public void Reload()
    {
        Initialize();
    }

    public async Task<HttpResponseMessage> SendMessageAsync(HttpRequestMessage request)
    {
        Console.WriteLine($"Sending message via HttpAdapter...");
        return await _apiClient.SendMessageAsync(request);
    }
    
    public async Task<string> HandleIncomingRequest(HttpContext context)
    {
        var requestMessage = new HttpRequestMessage(new HttpMethod(GetMethod(esbRoute.SendLocation) ?? string.Empty), GetUri(esbRoute.SendLocation))
        {
            Content = new StringContent(await new StreamReader(context.Request.Body).ReadToEndAsync())
        };

        var response = await SendMessageAsync(requestMessage);
        context.Response.StatusCode = (int)response.StatusCode;
        var responseContent = await response.Content.ReadAsStringAsync();
        await context.Response.WriteAsync(responseContent); //In Case of a REST to REST
        return responseContent; // Return it for the Consumer to map it in Bus Consumer
    }

    private static string? GetMethod(SendLocation? sendLocation)
    {
        if (sendLocation?.HttpEndpoint is not null)
        {
            return sendLocation.HttpEndpoint.Method;
        }
        else if (sendLocation?.FtpEndpoint is not null)
        {
            throw new NotSupportedException($"Client type '{sendLocation.FtpEndpoint.GetType()}' is not supported.");
        }

        throw new Exception("Check Http Method");
    }

    private static string? GetUri(SendLocation? sendLocation)
    {
        if (sendLocation?.HttpEndpoint is not null)
        {
            return sendLocation.HttpEndpoint.Url;
        }
        else if (sendLocation?.FtpEndpoint is not null)
        {
            throw new NotSupportedException($"Client type '{sendLocation.FtpEndpoint.GetType()}' is not supported.");
        }

        throw new Exception("Check Http Uri");
    }
}