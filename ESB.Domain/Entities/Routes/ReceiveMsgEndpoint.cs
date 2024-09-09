namespace ESB.Domain.Entities.Routes;

public class ReceiveMsgEndpoint
{
    public string? Assembly { get; set; }
    public string? EventType { get; set; }
    public string? CallbackResponseType { get; set; }
}