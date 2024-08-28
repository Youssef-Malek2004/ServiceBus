namespace ESB.Configurations.Routes;

public sealed class RouteSettings
{
    public string? ErrorHandlingProfile { get; set; }
    public RetryPolicy? RetryPolicyProfile { get; set; }
    public bool Sync { get; set; } = true;
}