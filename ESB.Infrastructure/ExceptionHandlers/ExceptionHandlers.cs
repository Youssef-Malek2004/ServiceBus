using ESB.Application.CustomExceptions;
using Microsoft.Extensions.Logging;

namespace ESB.Infrastructure.ExceptionHandlers;

public static class ExceptionHandlers
{
    public static Task Handle(Exception exception, ILogger<ExceptionHandlingMiddleware> logger) //ToDo Ext method
    {

        if (exception is MissingConfigurationException missingConfigurationException)
        {
            logger.LogError(missingConfigurationException,"Missing Configuration: {@ExceptionMessage} , Route: {@RouteId}",
                missingConfigurationException.Message,
                missingConfigurationException.RouteId);
        }
        else if (exception is MajorConfigurationException majorConfigurationException) //toDo pass exception to logger
        {
            logger.LogError(majorConfigurationException,"Major Configuration Exception: {@ExceptionMessage}",
                majorConfigurationException.Message);
        }
        else if (exception is GeneralRouteException generalRouteException)
        {
            logger.LogError(generalRouteException,"General Route Exception: {@ExceptionMessage} , Route: {@RouteId}",
                generalRouteException.Message,
                generalRouteException.RouteId);
        }
        else
        {
            logger.LogError(exception,"Internal Server Exception: {@ExceptionMessage}",
                exception.Message);
        }
        
        return Task.CompletedTask;
    }
}