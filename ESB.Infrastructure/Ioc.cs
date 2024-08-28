using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.Infrastructure.Configurators;
using ESB.Infrastructure.Services;
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
}