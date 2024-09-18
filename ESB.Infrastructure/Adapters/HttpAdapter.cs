using System.Net.Http.Headers;
using ESB.Application.CustomExceptions;
using ESB.Application.Interfaces;
using ESB.Application.Routes;
using ESB.Infrastructure.Factories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace ESB.Infrastructure.Adapters;

public class HttpAdapter(EsbRoute esbRoute, ClientFactory clientFactory, IHttpClientFactory httpClientFactory, ILogger<IAdapterDi> logger, IAuthorizer<HttpClient> authorizer) : IAdapterDi, IAdapter<HttpRequestMessage, HttpResponseMessage,StringValues, HttpContext>
{
     private readonly dynamic _apiClient = clientFactory.CreateClient(esbRoute.SendLocation, httpClientFactory, logger, authorizer);
     
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

    public async Task<string> HandleIncomingRequest(HttpContext context, StringValues authorization)
    {
        var uriBuilder = new UriBuilder(GetUri(esbRoute.SendLocation) ?? string.Empty)
        {
            Query = context.Request.QueryString.ToUriComponent()
        };
        
        var requestMessage = new HttpRequestMessage(new HttpMethod(this.GetMethod(esbRoute.SendLocation) ?? string.Empty), uriBuilder.Uri)
        {
            Content = new StringContent(await new StreamReader(context.Request.Body).ReadToEndAsync())
        };

        if (esbRoute.ReceiveLocation.Credentials.AuthorizationType.Equals("Bearer"))
        {
            var token = authorization.ToString().Replace("Bearer ", string.Empty); 
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);    
        }
        else if (esbRoute.ReceiveLocation.Credentials.AuthorizationType.Equals("Basic"))
        {
            var token = authorization.ToString().Replace("Basic ", string.Empty); 
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
        }
        
        
        var response = await SendMessageAsync(requestMessage);
        if (response.IsSuccessStatusCode)
        {
            context.Response.StatusCode = (int)response.StatusCode;
            var responseContent = await response.Content.ReadAsStringAsync();
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(responseContent);
            return responseContent;
        }
        else
        {
            logger.LogError("Request on Route :{@RouteId}  failed with status code: {@StatusCode}",
                esbRoute.Id,
                response.StatusCode);
            context.Response.StatusCode = (int)response.StatusCode;
            var responseContent = await response.Content.ReadAsStringAsync();
            await context.Response.WriteAsync(responseContent);
            return "Unauthorized";
        }
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