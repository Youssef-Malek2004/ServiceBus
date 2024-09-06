using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.ErrorHandling.CustomExceptions;
using ESB.Infrastructure.Factories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ESB.Infrastructure.Adapters;

public class HttpAdapter(EsbRoute esbRoute, ClientFactory clientFactory, IHttpClientFactory httpClientFactory, ILogger<IAdapterDi> logger, IAuthorizer<HttpClient> authorizer) : IAdapterDi, IAdapter<HttpRequestMessage, HttpResponseMessage, HttpContext>
{
     private readonly dynamic _apiClient = clientFactory.CreateClient(esbRoute.SendLocation, httpClientFactory, logger, authorizer); //toDo pass the correct auth
     
    public void Initialize()
    {
        _apiClient.Initialize();
        logger.LogInformation("Successful {@Adapter} initialization for Route : {@RouteId}",
            "HttpAdapter",
            esbRoute.Id);
    }

    public void Reload()
    {
        Initialize();
    }

    public async Task<HttpResponseMessage> SendMessageAsync(HttpRequestMessage request)
    {
        logger.LogInformation("{@Adapter} routing request {@Request} of Route : {@RouteId}",
            "HttpAdapter",
            request.RequestUri,
            esbRoute.Id);
        return await _apiClient.SendMessageAsync(request);
    }

    public async Task<string> HandleIncomingRequest(HttpContext context)
    {
        var uriBuilder = new UriBuilder(GetUri(esbRoute.SendLocation) ?? string.Empty)
        {
            Query = context.Request.QueryString.ToUriComponent() // Append the query string
        };
        
        var requestMessage = new HttpRequestMessage(new HttpMethod(this.GetMethod(esbRoute.SendLocation) ?? string.Empty), uriBuilder.Uri)
        {
            Content = new StringContent(await new StreamReader(context.Request.Body).ReadToEndAsync())
        };
        
        var response = await SendMessageAsync(requestMessage);
        context.Response.StatusCode = (int)response.StatusCode;
        var responseContent = await response.Content.ReadAsStringAsync();
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(responseContent); //In Case of a REST to REST
        return responseContent; // Return it for the Consumer to map it in BusConsumer as a body
    }

    private string? GetMethod(SendLocation? sendLocation)
    {
        if (sendLocation?.HttpEndpoint is not null)
        {
            return sendLocation.HttpEndpoint.Method;
        }
        throw new GeneralRouteException("Invalid HttpEndpoint",esbRoute.Id);
    }

    private string? GetUri(SendLocation? sendLocation)
    {
        if (sendLocation?.HttpEndpoint is not null)
        {
            return sendLocation.HttpEndpoint.Url;
        }

        throw new GeneralRouteException("Invalid HttpEndpoint",esbRoute.Id);
    }
}