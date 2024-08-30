namespace ESB.Configurations.Routes
{
    public sealed class ReceiveLocation
    {
        public ReceiveHttpEndpoint? HttpEndpoint { get; set; }
        public ReceiveFtpEndpoint? FtpEndpoint { get; set; }
        public ReceiveMsgEndpoint? MessageEndpoint { get; set; }
    }

    public sealed class SendLocation
    {
        public SendHttpEndpoint? HttpEndpoint { get; set; }
        public SendFtpEndpoint? FtpEndpoint { get; set; }
    }
}