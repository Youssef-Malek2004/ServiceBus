using System.Collections.Concurrent;
using ESB.Application.Interfaces;
using ESB.Application.Routes;
using ESB.Infrastructure.Factories;
using ESB.Infrastructure.Services;
using Polly;
using Polly.RateLimiting;
using Polly.Retry;

namespace ESB.Core.Middlewares;

public static class CustomMiddlewares
{
    public static IApplicationBuilder AddHttpReceiveEndpoints(this IApplicationBuilder app)
    {
        var routesConfigurationService = app.ApplicationServices.GetRequiredService<RoutesConfigurationService>();
        var adapterDictionary = app.ApplicationServices.GetRequiredService<ConcurrentDictionary<EsbRoute, IAdapterDi>>();
        var resiliencyDictionary = app.ApplicationServices.GetRequiredService<ConcurrentDictionary<EsbRoute, ResiliencePipeline>>();
        HelperMiddlewares.AddHttpEndpoints(app, routesConfigurationService, adapterDictionary,resiliencyDictionary);
        
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

    public static IApplicationBuilder AddResiliency(this IApplicationBuilder app)
    {
        var routesConfigurationService = app.ApplicationServices.GetRequiredService<RoutesConfigurationService>();
        var resiliencyDictionary = app.ApplicationServices.GetRequiredService<ConcurrentDictionary<EsbRoute, ResiliencePipeline>>();

        if (routesConfigurationService.RoutesConfiguration.Routes == null) return app;
        foreach (var route in routesConfigurationService.RoutesConfiguration.Routes)
        {
            if(route.Settings?.RetryPolicy is null) continue;
            
            var routePipeline = new ResiliencePipelineBuilder() 
                .AddRetry(new RetryStrategyOptions()
                {
                    ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                    Delay = TimeSpan.FromSeconds(route.Settings.RetryPolicy.Delay),
                    MaxRetryAttempts = route.Settings.RetryPolicy.MaxRetryAttempts,
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true
                })
                .AddTimeout(TimeSpan.FromSeconds(route.Settings.RetryPolicy.Timeout))
                .Build();

            resiliencyDictionary.TryAdd(route, routePipeline);

        }

        return app;
    }
}