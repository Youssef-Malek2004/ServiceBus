using System.Collections.Concurrent;
using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.Infrastructure.Adapters;
using ESB.Infrastructure.Services;

namespace ESB.Core.Middlewares;

public static class HelperMiddlewares
{
    public static void AddHttpEndpoints(IApplicationBuilder app,
        RoutesConfigurationService routesConfigurationService,
        ConcurrentDictionary<EsbRoute, IAdapterDi> adapterDictionary)
    {
        app.UseEndpoints(endpoints =>
        {
            if (routesConfigurationService.RoutesConfiguration.Routes == null) return;

            foreach (var route in routesConfigurationService.RoutesConfiguration.Routes)
            {
                if (route.ReceiveLocation?.HttpEndpoint is null) continue;

                var receiveLocation = route.ReceiveLocation.HttpEndpoint;
                var method = receiveLocation.Method?.ToUpperInvariant();

                if (method == null) continue;

                switch (method)
                {
                    case "GET":
                        endpoints.MapGet($"/{receiveLocation.EndpointName}",
                            async (HttpContext context, HttpResponse response) =>
                            {
                                adapterDictionary.TryGetValue(route, out var adapter);
                                if (adapter is HttpAdapter httpAdapter)
                                {
                                    await httpAdapter.HandleIncomingRequest(context);
                                }
                            });
                        break;

                    case "POST":
                        endpoints.MapPost($"/{receiveLocation.EndpointName}",
                            async (HttpContext context, HttpResponse response) =>
                            {
                                adapterDictionary.TryGetValue(route, out var adapter);
                                if (adapter is HttpAdapter httpAdapter)
                                {
                                    await httpAdapter.HandleIncomingRequest(context);
                                }
                            });
                        break;

                    case "PUT":
                        endpoints.MapPut($"/{receiveLocation.EndpointName}",
                            async (HttpContext context, HttpResponse response) =>
                            {
                                adapterDictionary.TryGetValue(route, out var adapter);
                                if (adapter is HttpAdapter httpAdapter)
                                {
                                    await httpAdapter.HandleIncomingRequest(context);
                                }
                            });
                        break;

                    case "DELETE":
                        endpoints.MapDelete($"/{receiveLocation.EndpointName}",
                            async (HttpContext context, HttpResponse response) =>
                            {
                                adapterDictionary.TryGetValue(route, out var adapter);
                                if (adapter is HttpAdapter httpAdapter)
                                {
                                    await httpAdapter.HandleIncomingRequest(context);
                                }
                            });
                        break;
                    

                    default:
                        throw new InvalidOperationException($"Unsupported HTTP method: {method}");
                }
            }
        });
    }
}