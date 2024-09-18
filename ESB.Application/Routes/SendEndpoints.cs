namespace ESB.Application.Routes;

public class SendHttpEndpoint
{
    public string? Url { get; set; }
    public string? Method { get; set; }
    // public RouteAuthorization? Authorization { get; set; }
    public Credentials? Credentials { get; set; }
}

public class SendFtpEndpoint
{
    
}