using ESB.Infrastructure.Adapters;
using ESB.Infrastructure.Factories;
using ESB.Infrastructure.Services;

namespace ESB.Core.Middlewares;

public static class CustomMiddlewares
{
    public static IApplicationBuilder UseEndpointMapping(this IApplicationBuilder app,
        RoutesConfigurationService routesConfigurationService)
    {
        app.UseEndpoints(endpoints =>
        {
            if (routesConfigurationService.RoutesConfiguration.Routes != null)
                foreach (var route in routesConfigurationService.RoutesConfiguration.Routes)
                {
                    var receiveLocation = route.ReceiveLocation.HttpEndpoint;

                    endpoints.MapGet($"/{receiveLocation.EndpointName}",
                        async (HttpContext context, IHttpClientFactory httpClientFactory, ClientFactory clientFactory) =>
                        {
                            var adapter = new HttpAdapter(route, clientFactory, httpClientFactory);

                            adapter.Initialize();
                            await adapter.HandleIncomingRequest(context);
                        });
                }
        });
        return app;
    }
}