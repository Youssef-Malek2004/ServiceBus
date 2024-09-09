using System.Text.Json.Serialization;
using Microsoft.Extensions.Primitives;

namespace ESB.Messages.Interfaces;

public interface ICoreMessage
{
    public string? Authorization { get; set; }
}