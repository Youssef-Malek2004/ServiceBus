namespace ESB.ErrorHandling.CustomExceptions;

public class EsbConfigurationExceptionConsumer(string message, string? routeId) : Exception(message)
{
    public string? RouteId { get; set; } = routeId;
    
}