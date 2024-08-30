using System.Collections.Concurrent;
using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.Infrastructure.Adapters;
using ESB.Infrastructure.Services;
using MassTransit;
using MassTransit.Internals;
using Microsoft.AspNetCore.Http;

namespace ESB.Infrastructure.Consumers;

public class BusConsumer<TMessage, TResponse>(
    IPublishEndpoint publishEndpoint,
    RoutesConfigurationService routesConfigurationService,
    ConcurrentDictionary<EsbRoute, IAdapterDi> adapterDictionary)
    : IBusConsumerNonGen, IBusConsumer<TMessage, TResponse>
    where TMessage : class
{
    //ToDO Validate Config to check  
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private EsbRoute? EsbRoute { get; set; }

    public async Task Consume(ConsumeContext<TMessage> context)
    {
        if (routesConfigurationService.RoutesConfiguration.Routes != null && EsbRoute is null)
            foreach (var route in routesConfigurationService.RoutesConfiguration.Routes.Where(route => route.ReceiveLocation?.MessageEndpoint is not null))
            {
                if(route.ReceiveLocation?.MessageEndpoint?.EventType is null) throw new Exception($"Missing Event Type in message endpoint with Route id: {route.Id}"); //ToDo Logging
                if (route.ReceiveLocation.MessageEndpoint.EventType.Equals(context.Message.GetType().GetTypeName()))
                {
                    EsbRoute = route;
                }
            }

        if (EsbRoute is null) throw new Exception(); //ToDo not exception use Logging
        adapterDictionary.TryGetValue(EsbRoute, out var adapter);
         
        if (adapter is HttpAdapter httpAdapter)
        {
         await httpAdapter.HandleIncomingRequest(new DefaultHttpContext());
        }
         
        Console.WriteLine($"Well Done Soldier -> Consumed Message! {context.Message.GetType()}");
        await Task.CompletedTask;
    }
}