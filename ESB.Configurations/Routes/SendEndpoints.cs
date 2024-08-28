namespace ESB.Configurations.Routes;

public class SendHttpEndpoint
{
    public string? Url { get; set; }
    public string? Method { get; set; }
    public string? AuthorizationType { get; set; }

    public AuthorizationParameters? AuthorizationParameters { get; set; }
}

public class SendFtpEndpoint
{
    
}