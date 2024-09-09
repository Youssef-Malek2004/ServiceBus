using ESB.Messages.Interfaces;
using Microsoft.Extensions.Primitives;

namespace ESB.Messages.Events;

public class MohamedMessage : ICoreMessage
{
    public string? Name { get; set; }
    public string? Authorization { get; set; }
    
}