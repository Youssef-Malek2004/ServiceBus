namespace ESB.ErrorHandling.CustomExceptions;

public class GeneralRouteException(string message, string? routeId) : Exception(message)
{
    public string? RouteId { get; set; } = routeId;
}