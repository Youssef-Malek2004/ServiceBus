namespace ESB.Domain.Entities.Routes
{
    public sealed class RoutesSettings
    {
        public string? ErrorHandlingProfile { get; set; }
        public string? RetryPolicyProfile { get; set; }
        public bool Sync { get; set; }
    }
}