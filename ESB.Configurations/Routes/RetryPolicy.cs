namespace ESB.Configurations.Routes
{
    public sealed class RetryPolicy
    {
        public int MaxRetries { get; set; } = 3;
        public int RetryIntervalSeconds { get; set; } = 5;
    }
}