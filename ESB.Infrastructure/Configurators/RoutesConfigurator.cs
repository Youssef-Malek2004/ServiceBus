using ESB.Application.CustomExceptions;
using ESB.Application.Interfaces;
using ESB.Application.Routes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ESB.Infrastructure.Configurators;

public class RoutesConfigurator(IConfiguration configuration, ILogger<RoutesConfigurator> logger) : IConfigurator<ConfiguredRoutes>
{
    public ConfiguredRoutes InitializeConfiguration()
    {
        var configuredRoutes = new ConfiguredRoutes
        {
            Routes = configuration.GetSection("Routes").Get<List<EsbRoute>>(),
            DefaultSettings = configuration.GetSection("DefaultSettings").Get<RoutesSettings>()
        };
        logger.LogInformation("Successfully Read the Specified Routes!");
        
        return configuredRoutes ?? throw new MajorConfigurationException("Empty Routes Section");
    }

    public ConfiguredRoutes ReloadConfiguration()
    {
        // Reload or reinitialize the configuration 
        return InitializeConfiguration(); 
    }

    public bool ValidateConfiguration()
    {
        return true;
    }
}