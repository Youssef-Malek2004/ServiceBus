using ESB.Application.Routes;

namespace ESB.Infrastructure.Services;

public class RoutesConfigurationService
{
    public ConfiguredRoutes RoutesConfiguration { get; set; }

    public RoutesConfigurationService(ConfiguredRoutes routesConfiguration)
    {
        RoutesConfiguration = routesConfiguration;
    }
}