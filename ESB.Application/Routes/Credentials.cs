namespace ESB.Application.Routes;

public class Credentials
{
    public string? AuthorizationType { get; set; }
    public BearerTokenCredentials? BearerTokenCredentials { get; set; }
    public BasicCredentials? BasicCredentials { get; set; }
}