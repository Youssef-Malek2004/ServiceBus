using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.ErrorHandling.CustomExceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        //ToDo validate the configuration
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
        throw new NotImplementedException();
    }
}