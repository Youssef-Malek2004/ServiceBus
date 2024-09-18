namespace ESB.Application.CustomExceptions;

public class MissingConfigurationException(string message, string? routeId) : Exception(message)
{
    public string? RouteId { get; set; } = routeId;
}