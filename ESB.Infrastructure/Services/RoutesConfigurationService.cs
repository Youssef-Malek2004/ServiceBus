using ESB.Configurations.Routes;

namespace ESB.Infrastructure.Services;

public class RoutesConfigurationService
{
    public ConfiguredRoutes RoutesConfiguration { get; private set; }

    public RoutesConfigurationService(ConfiguredRoutes routesConfiguration)
    {
        RoutesConfiguration = routesConfiguration;
    }
}