using ESB.Messages.Interfaces;
namespace ESB.Messages.Events;

public class MohamedMessage : ICoreMessage
{
    public string? Name { get; set; }
    public string? Authorization { get; set; }
    
}