using ESB.Configurations.Interfaces;
using ESB.ErrorHandling.CustomExceptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ESB.Infrastructure.ExceptionHandlers;

public class GlobalConsumerObserver(ILogger<GlobalConsumerObserver> logger) : IGlobalConsumerObserver
{
    public Task PreConsume<T>(ConsumeContext<T> context) where T : class
    {
        logger.LogInformation("Started Consuming message {@Message} at {@DateTimeUtc}",
            context.Message,
            DateTime.UtcNow);
        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context) where T : class
    {
        logger.LogInformation("Finished Consuming message successfully {@Message} at {@DateTimeUtc}",
            context.Message,
            DateTime.UtcNow);
        return Task.CompletedTask;
    }

    public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
    {
        if (exception is MissingConfigurationException missingConfigurationException)
        {
            logger.LogError(missingConfigurationException,"Missing Configuration: {@ExceptionMessage} of Message: {@MessageType} in Route: {@RouteId}",
               missingConfigurationException.Message,
               context.Message,
               missingConfigurationException.RouteId);
        }
        else
        {
            logger.LogError(exception,"Missing Configuration: {@ExceptionMessage} of Message: {@MessageType}",
                exception.Message,
                context.Message);
        }

        return Task.CompletedTask;
    }
}