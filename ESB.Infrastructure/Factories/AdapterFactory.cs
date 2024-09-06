using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.ErrorHandling.CustomExceptions;
using ESB.Infrastructure.Adapters;
using Microsoft.Extensions.Logging;

namespace ESB.Infrastructure.Factories;

public static class AdapterFactory
{
    public static IAdapterDi CreateAdapter(EsbRoute esbRoute, IHttpClientFactory httpClientFactory, ClientFactory clientFactory, ILogger<IAdapterDi> adapterLogger, IAuthorizer<HttpClient> authorizer)
    {
        if (esbRoute.SendLocation?.HttpEndpoint is not null)
        {
            var adapter = new HttpAdapter(esbRoute, clientFactory, httpClientFactory, adapterLogger, authorizer); //todo pass the correct auth
            adapter.Initialize();
            return adapter;
        }

        throw new GeneralRouteException("Adapter is not supported", esbRoute.Id);
    }
}