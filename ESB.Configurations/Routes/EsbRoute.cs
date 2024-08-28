namespace ESB.Configurations.Routes;

public sealed class EsbRoute
{
    public string? Id { get; set; }
    public bool Enabled { get; set; }
    public ReceiveLocation? ReceiveLocation { get; set; }
    public SendLocation? SendLocation { get; set; }
    public RouteSettings? Settings { get; set; }
}