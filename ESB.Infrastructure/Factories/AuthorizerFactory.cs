using ESB.Application.Interfaces;
using ESB.Application.Routes;
using ESB.Infrastructure.Authorizers;
using ESB.Infrastructure.Clients;
using Microsoft.Extensions.Logging;

namespace ESB.Infrastructure.Factories;

public static class AuthorizerFactory
{
    public static IAuthorizer<HttpClient> CreateAuthorizer(SendLocation? sendLocation, ILogger<IAdapterDi> logger)
    {
        //     if (sendLocation?.HttpEndpoint is not null)
        //     {
        //         if (sendLocation.HttpEndpoint.Credentials != null && sendLocation.HttpEndpoint.Credentials.AuthorizationType == "Basic")
        //         {
        //             if (sendLocation.HttpEndpoint.Credentials.BasicCredentials != null)
        //                 return new BasicAuthorizer(sendLocation.HttpEndpoint.Credentials.BasicCredentials);
        //         }
        //         else if (sendLocation.HttpEndpoint.Credentials != null &&
        //                  sendLocation.HttpEndpoint.Credentials.AuthorizationType == "Bearer")
        //         {
        //             if (sendLocation.HttpEndpoint.Credentials.BearerTokenCredentials != null)
        //                 return new BearerAuthorizer(sendLocation.HttpEndpoint.Credentials.BearerTokenCredentials);
        //         }
        //     }
        //     else if (sendLocation?.FtpEndpoint is not null)
        //     {
        //         throw new NotSupportedException($"Client type '{sendLocation.FtpEndpoint.GetType()}' is not supported.");
        //     }
        //
        //     throw new EsbRuntimeException("Could not find a defined Send Endpoint");
        // }
        return new BasicAuthorizer(null);
    }
}