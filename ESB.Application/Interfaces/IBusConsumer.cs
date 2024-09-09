using MassTransit;

namespace ESB.Application.Interfaces;

public interface IBusConsumer<in TMessage, TResponse> :  IConsumer<TMessage> where TMessage : class
{
    
}