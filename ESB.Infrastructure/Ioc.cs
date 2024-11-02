using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using ESB.Application.CustomExceptions;
using ESB.Application.Interfaces;
using ESB.Application.Routes;
using ESB.Infrastructure.Configurators;
using ESB.Infrastructure.ExceptionHandlers;
using ESB.Infrastructure.Factories;
using ESB.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace ESB.Infrastructure;

public static class Ioc
{
    public static IServiceCollection AddConfigurationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConfiguredRoutes>(
            configuration.GetSection("BusConfiguration"));
        
        // services.AddTransient<IConfigurator<ConfiguredRoutes>, RoutesConfigurator>();
        // services.AddTransient(provider =>
        // {
        //     var routesConfigurator = provider.GetRequiredService<IConfigurator<ConfiguredRoutes>>();
        //     var routesConfiguration = routesConfigurator.InitializeConfiguration();
        //     return new RoutesConfigurationService(routesConfiguration);
        // });
        
        return services;
    }

    public static IServiceCollection AddRequiredComponents(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<ClientFactory>();
        services.AddSingleton<ConcurrentDictionary<EsbRoute, IAdapterDi>>();
        services.AddSingleton<ConcurrentDictionary<EsbRoute, ResiliencePipeline>>();
        return services;
    }

    public static IServiceCollection ConfigureBus(this IServiceCollection services, ConfigurationManager configurationManager)
    { 
        var serviceProvider = services.BuildServiceProvider();
        var loggerExceptionHandler = serviceProvider.GetRequiredService<ILogger<ExceptionHandlingMiddleware>>();
        var loggerConsumerObserver = serviceProvider.GetRequiredService<ILogger<GlobalConsumerObserver>>();

        try
        {
            // var routesConfigService = serviceProvider.GetRequiredService<RoutesConfigurationService>();
            var routesConfigService = serviceProvider.GetRequiredService<IOptions<ConfiguredRoutes>>();

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                if (routesConfigService.Value.Routes != null)
                    foreach (var route in routesConfigService.Value.Routes)
                    {
                        if (route.ReceiveLocation?.MessageEndpoint is null) continue;
                        if (route.ReceiveLocation.MessageEndpoint.EventType == null)
                            throw new MissingConfigurationException("Missing Event Type", route.Id);
                        
                        if (route.ReceiveLocation.MessageEndpoint.CallbackResponseType == null)
                            throw new MissingConfigurationException("Missing Response Type", route.Id);
                        
                        if (route.ReceiveLocation.MessageEndpoint.Assembly == null)
                            throw new MissingConfigurationException("Missing Assembly", route.Id);
                        var currentAssembly = Assembly.Load(route.ReceiveLocation.MessageEndpoint.Assembly);
                        var myEventType = currentAssembly.GetType(route.ReceiveLocation.MessageEndpoint.EventType);
                        var myResponseType =
                            currentAssembly.GetType(route.ReceiveLocation.MessageEndpoint.CallbackResponseType);

                        if (myEventType == null || myResponseType == null)
                            throw new MajorConfigurationException("Can't Find Event or Response Type in Assembly");
                        var busConsumer = ConsumerFactory.Create(myEventType, myResponseType);

                        busConfigurator.AddConsumer(busConsumer); 
                    }

                busConfigurator.UsingRabbitMq((context, configurator) =>
                {
                    configurator.ConnectConsumeObserver(new GlobalConsumerObserver(loggerConsumerObserver));
                    
                    configurator.ClearSerialization();
                    configurator.UseRawJsonSerializer();

                    configurator.Host(new Uri(configurationManager["MessageBroker:Host"]!), h =>
                    {
                        h.Username(configurationManager["MessageBroker:Username"]!);
                        h.Password(configurationManager[
                            "MessageBroker:Password"]!);
                    });
                    configurator.ConfigureEndpoints(context);
                });
            });

        }
        catch (Exception exception)
        {
            ExceptionHandlers.ExceptionHandlers.Handle(exception,loggerExceptionHandler);
            Environment.Exit(-1);
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