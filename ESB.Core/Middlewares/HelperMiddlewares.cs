using System.Collections.Concurrent;
using System.Net.Http.Headers;
using ESB.Application.Interfaces;
using ESB.Application.Routes;
using ESB.Infrastructure.Adapters;
using ESB.Infrastructure.Services;
using Microsoft.Extensions.Primitives;
using Polly;

namespace ESB.Core.Middlewares;

public static class HelperMiddlewares
{
    public static void AddHttpEndpoints(IApplicationBuilder app,
        RoutesConfigurationService routesConfigurationService,
        ConcurrentDictionary<EsbRoute, IAdapterDi> adapterDictionary,
        ConcurrentDictionary<EsbRoute, ResiliencePipeline> resiliencyDictionary)
    {
        app.UseEndpoints(endpoints =>
        {
            if (routesConfigurationService.RoutesConfiguration.Routes == null) return;

            foreach (var route in routesConfigurationService.RoutesConfiguration.Routes)
            {
                if (route.ReceiveLocation?.HttpEndpoint is null) continue;

                var receiveLocation = route.ReceiveLocation.HttpEndpoint;
                var method = receiveLocation.Method?.ToUpperInvariant();

                resiliencyDictionary.TryGetValue(route, out var resiliencePipeline);

                if (method == null) continue;

                switch (method)
                {
                    case "GET":
                        endpoints.MapGet($"/{receiveLocation.EndpointName}",
                            async (HttpContext context, HttpResponse response) =>
                            {
                                StringValues test = context.Request.Headers.Authorization;
                                adapterDictionary.TryGetValue(route, out var adapter);
                                if (adapter is HttpAdapter httpAdapter)
                                {
                                    if (resiliencePipeline != null)
                                        await resiliencePipeline.ExecuteAsync(async cancellationToken =>
                                            await httpAdapter.HandleIncomingRequest(context, context.Request.Headers.Authorization));
                                    else
                                    {
                                        await httpAdapter.HandleIncomingRequest(context, context.Request.Headers.Authorization);
                                    }
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
                                    if (resiliencePipeline != null)
                                        await resiliencePipeline.ExecuteAsync(async cancellationToken =>
                                            await httpAdapter.HandleIncomingRequest(context, context.Request.Headers.Authorization));
                                    else
                                    {
                                        await httpAdapter.HandleIncomingRequest(context, context.Request.Headers.Authorization);
                                    }
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
                                    if (resiliencePipeline != null)
                                        await resiliencePipeline.ExecuteAsync(async cancellationToken =>
                                            await httpAdapter.HandleIncomingRequest(context, context.Request.Headers.Authorization));
                                    else
                                    {
                                        await httpAdapter.HandleIncomingRequest(context, context.Request.Headers.Authorization);
                                    }
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
                                    if (resiliencePipeline != null)
                                        await resiliencePipeline.ExecuteAsync(async cancellationToken =>
                                            await httpAdapter.HandleIncomingRequest(context, context.Request.Headers.Authorization));
                                    else
                                    {
                                        await httpAdapter.HandleIncomingRequest(context, context.Request.Headers.Authorization);
                                    }
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