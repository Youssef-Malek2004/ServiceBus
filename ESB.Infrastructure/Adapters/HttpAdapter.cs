using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.Core.Interfaces;
using ESB.Infrastructure.Factories;
using ESB.Infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace ESB.Infrastructure.Adapters;

public class HttpAdapter(EsbRoute esbRoute, ClientFactory clientFactory, IHttpClientFactory httpClientFactory) : IAdapter<HttpRequestMessage, HttpResponseMessage, HttpContext>
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
    
    public async Task HandleIncomingRequest(HttpContext context)
    {
        var requestMessage = new HttpRequestMessage(new HttpMethod(GetMethod(esbRoute.SendLocation) ?? string.Empty), GetUri(esbRoute.SendLocation))
        {
            Content = new StringContent(await new StreamReader(context.Request.Body).ReadToEndAsync())
        };

        var response = await SendMessageAsync(requestMessage);
        context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
    }

    public static string? GetMethod(SendLocation? sendLocation)
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

    public static string? GetUri(SendLocation? sendLocation)
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