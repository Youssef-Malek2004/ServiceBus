using ESB.Messages.Interfaces;
using Microsoft.Extensions.Primitives;

namespace ESB.Messages.Events;

public class TestEvent : ICoreMessage
{
    public int Num { get; set; }
    public string? Authorization { get; set; }
    
}