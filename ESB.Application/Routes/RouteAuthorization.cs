namespace ESB.Application.Routes;

public class RouteAuthorization
{
    public string? AuthorizationType { get; set; }
    public BasicAuthorization? BasicAuthorization { get; set; }
    public BearerTokenAuthorization? BearerTokenAuthorization { get; set; }
    public Credentials? Credentials { get; set; }
}