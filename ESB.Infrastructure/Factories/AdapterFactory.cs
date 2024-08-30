using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.Infrastructure.Adapters;

namespace ESB.Infrastructure.Factories;

public static class AdapterFactory
{
    public static IAdapterDi CreateAdapter(EsbRoute esbRoute, IHttpClientFactory httpClientFactory, ClientFactory clientFactory)
    {
        if (esbRoute.SendLocation?.HttpEndpoint is not null)
        {
            var adapter = new HttpAdapter(esbRoute, clientFactory, httpClientFactory);
            adapter.Initialize();
            return adapter;
        }

        throw new Exception("Adapter for this type does not exist");
    }
}