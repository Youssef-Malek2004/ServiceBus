using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.Infrastructure.Clients;

namespace ESB.Infrastructure.Factories;

public class ClientFactory
{
    public object CreateClient(SendLocation? sendLocation, IHttpClientFactory httpClientFactory) 
    {
        if (sendLocation?.HttpEndpoint is not null)
        {
            return new HttpApiClient(httpClientFactory);
        }
        else if (sendLocation?.FtpEndpoint is not null)
        {
            throw new NotSupportedException($"Client type '{sendLocation.FtpEndpoint.GetType()}' is not supported.");
        }

        return false;
    }
}