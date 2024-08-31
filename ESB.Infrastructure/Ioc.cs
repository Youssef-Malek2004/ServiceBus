using System.Collections.Concurrent;
using System.Reflection;
using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.Infrastructure.Configurators;
using ESB.Infrastructure.Factories;
using ESB.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ESB.Infrastructure;

public static class Ioc
{
    public static IServiceCollection AddConfigurationService(this IServiceCollection services)
    {
        services.AddSingleton<IConfigurator<ConfiguredRoutes>, RoutesConfigurator>();
        services.AddSingleton(provider =>
        {
            var routesConfigurator = provider.GetRequiredService<IConfigurator<ConfiguredRoutes>>();
            var routesConfiguration = routesConfigurator.InitializeConfiguration();
            return new RoutesConfigurationService(routesConfiguration);
        });
        
        return services;
    }

    public static IServiceCollection AddRequiredComponents(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<ClientFactory>();
        services.AddSingleton<ConcurrentDictionary<EsbRoute, IAdapterDi>>();
        return services;
    }

    public static IServiceCollection ConfigureBus(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        var serviceProvider = services.BuildServiceProvider();
        
        try
        {
            var routesConfigService = serviceProvider.GetRequiredService<RoutesConfigurationService>();
            
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                if (routesConfigService.RoutesConfiguration.Routes != null)
                    foreach (var route in routesConfigService.RoutesConfiguration.Routes)
                    {
                        if (route.ReceiveLocation?.MessageEndpoint is null) continue;
                        if (route.ReceiveLocation.MessageEndpoint.Assembly == null
                            || route.ReceiveLocation.MessageEndpoint.EventType == null
                            || route.ReceiveLocation.MessageEndpoint.ResponseType == null) throw new Exception("Missing Fields");
                        var currentAssembly = Assembly.Load(route.ReceiveLocation.MessageEndpoint.Assembly);
                        var myEventType = currentAssembly.GetType(route.ReceiveLocation.MessageEndpoint.EventType);
                        var myResponseType = currentAssembly.GetType(route.ReceiveLocation.MessageEndpoint.ResponseType);
                        
                        if (myEventType == null || myResponseType == null) throw new Exception("Missing Parameters");
                        var busConsumer = ConsumerFactory.Create(myEventType, myResponseType);
                        
                        busConfigurator.AddConsumer(busConsumer);
                    }
                // busConfigurator.AddConsumer<TestConsumer>(); //Testing the Consumer for Response Message -> Published Successfully
                
                busConfigurator.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(configurationManager["MessageBroker:Host"]!), h =>
                    {
                        h.Username(configurationManager["MessageBroker:Username"]!);
                        h.Password(configurationManager["MessageBroker:Password"]!); //ToDo Change this to be included in the routes config
                    });

                    configurator.ConfigureEndpoints(context);
                });
            });

        }
        finally
        {
            if (serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        
        return services;
    }
}