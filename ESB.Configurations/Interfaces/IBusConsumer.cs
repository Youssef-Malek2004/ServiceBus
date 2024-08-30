using ESB.Configurations.Routes;
using MassTransit;

namespace ESB.Configurations.Interfaces;

public interface IBusConsumer<in TMessage, TResponse> :  IConsumer<TMessage> where TMessage : class
{
    
}