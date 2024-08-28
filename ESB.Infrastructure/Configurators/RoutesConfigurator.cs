using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ESB.Infrastructure.Configurators;

public class RoutesConfigurator(IConfiguration configuration) : IConfigurator<ConfiguredRoutes>
{
    public ConfiguredRoutes InitializeConfiguration()
    {
        var configuredRoutes = new ConfiguredRoutes
        {
            Routes = configuration.GetSection("Routes").Get<List<EsbRoute>>(),
            DefaultSettings = configuration.GetSection("DefaultSettings").Get<RoutesSettings>()
        };
        return configuredRoutes ?? throw new Exception("Empty Routes Section");
    }

    public ConfiguredRoutes ReloadConfiguration()
    {
        // Reload or reinitialize the configuration 
        return InitializeConfiguration(); 
    }
}