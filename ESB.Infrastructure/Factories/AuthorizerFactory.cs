using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.ErrorHandling.CustomExceptions;
using ESB.Infrastructure.Authorizers;
using ESB.Infrastructure.Clients;
using Microsoft.Extensions.Logging;

namespace ESB.Infrastructure.Factories;

public static class AuthorizerFactory
{
    public static IAuthorizer<HttpClient> CreateAuthorizer(SendLocation? sendLocation, ILogger<IAdapterDi> logger) 
    {
        if (sendLocation?.HttpEndpoint is not null)
        {
            if (sendLocation.HttpEndpoint.Authorization != null && sendLocation.HttpEndpoint.Authorization.AuthorizationType == "Basic")
            {
                if (sendLocation.HttpEndpoint.Authorization.BasicAuthorization != null)
                    return new BasicAuthorizer(sendLocation.HttpEndpoint.Authorization.BasicAuthorization);
            }
            else if (sendLocation.HttpEndpoint.Authorization != null &&
                     sendLocation.HttpEndpoint.Authorization.AuthorizationType == "Bearer")
            {
                if (sendLocation.HttpEndpoint.Authorization.BearerTokenAuthorization != null)
                    return new BearerAuthorizer(sendLocation.HttpEndpoint.Authorization.BearerTokenAuthorization);
            }
        }
        else if (sendLocation?.FtpEndpoint is not null)
        {
            throw new NotSupportedException($"Client type '{sendLocation.FtpEndpoint.GetType()}' is not supported.");
        }

        throw new EsbRuntimeException("Could not find a defined Send Endpoint");
    }
}