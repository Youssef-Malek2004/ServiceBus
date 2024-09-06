using System.Collections.Concurrent;
using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.Infrastructure.Authorizers;
using ESB.Infrastructure.Factories;
using ESB.Infrastructure.Services;

namespace ESB.Core.Middlewares;

public static class CustomMiddlewares
{
    public static IApplicationBuilder AddHttpReceiveEndpoints(this IApplicationBuilder app)
    {
        var routesConfigurationService = app.ApplicationServices.GetRequiredService<RoutesConfigurationService>();
        var adapterDictionary = app.ApplicationServices.GetRequiredService<ConcurrentDictionary<EsbRoute, IAdapterDi>>();
        HelperMiddlewares.AddHttpEndpoints(app, routesConfigurationService, adapterDictionary);
        
        return app;
    }

    public static IApplicationBuilder AddAdapters(this IApplicationBuilder app)
    {
        var routesConfigurationService = app.ApplicationServices.GetRequiredService<RoutesConfigurationService>();
        var httpClientFactory = app.ApplicationServices.GetRequiredService<IHttpClientFactory>();
        var clientFactory = app.ApplicationServices.GetRequiredService<ClientFactory>();
        var adapterDictionary = app.ApplicationServices.GetRequiredService<ConcurrentDictionary<EsbRoute, IAdapterDi>>();
        var adapterLogger = app.ApplicationServices.GetRequiredService<ILogger<IAdapterDi>>();
        
        if (routesConfigurationService.RoutesConfiguration.Routes == null) return app;
        foreach (var route in routesConfigurationService.RoutesConfiguration.Routes)
        {
            var clientAuthorizer = AuthorizerFactory.CreateAuthorizer(route.SendLocation, adapterLogger);
            var adapter = AdapterFactory.CreateAdapter(route, httpClientFactory, clientFactory, adapterLogger, clientAuthorizer);
            adapterDictionary.TryAdd(route, adapter);
        }

        return app;
    }
}