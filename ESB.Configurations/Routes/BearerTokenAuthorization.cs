namespace ESB.Configurations.Routes;

public class BearerTokenAuthorization
{
    public string? Token { get; set; }
    public string? LoginUrl { get; set; }
    public int RefreshTokenInterval { get; set; } 
}