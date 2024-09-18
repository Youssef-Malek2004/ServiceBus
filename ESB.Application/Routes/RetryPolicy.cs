namespace ESB.Application.Routes
{
    public sealed class RetryPolicy
    {
        public int Delay { get; set; } = 2;
        public int MaxRetryAttempts { get; set; } = 1;
        public int Timeout { get; set; } = 30;
    }
}