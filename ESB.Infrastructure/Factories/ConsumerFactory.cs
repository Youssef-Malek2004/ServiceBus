using ESB.Infrastructure.Consumers;

namespace ESB.Infrastructure.Factories;

public static class ConsumerFactory
{
    public static Type? Create(Type messageType, Type responseType)
    {
        // Get the type of MyConsumer<TMessage, TResponse>
        Type genericConsumerType = typeof(BusConsumer<,>);

        // Create a concrete type for MyConsumer<TMessage, TResponse>
        Type concreteConsumerType = genericConsumerType.MakeGenericType(messageType, responseType);

        // Create an instance of the concrete type
        return concreteConsumerType;
    }
}