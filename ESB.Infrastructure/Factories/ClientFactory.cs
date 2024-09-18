using ESB.Application.Interfaces;
using ESB.Application.Routes;
using ESB.Infrastructure.Clients;
using Microsoft.Extensions.Logging;

namespace ESB.Infrastructure.Factories;

public class ClientFactory
{
    public object CreateClient(SendLocation? sendLocation, IHttpClientFactory httpClientFactory, ILogger<IAdapterDi> logger, IAuthorizer<HttpClient> authorizer) 
    {
        if (sendLocation?.HttpEndpoint is not null)
        {
            return new HttpApiClient(httpClientFactory, logger);
        }
        else if (sendLocation?.FtpEndpoint is not null)
        {
            throw new NotSupportedException($"Client type '{sendLocation.FtpEndpoint.GetType()}' is not supported.");
        }

        return false;
    }
}