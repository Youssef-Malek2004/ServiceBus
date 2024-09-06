using System.Collections.Concurrent;
using System.Reflection;
using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.ErrorHandling.CustomExceptions;
using ESB.Infrastructure.Adapters;
using ESB.Infrastructure.Services;
using MassTransit;
using MassTransit.Internals;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ESB.Infrastructure.Consumers;

public class BusConsumer<TMessage, TResponse>(
    IPublishEndpoint publishEndpoint,
    RoutesConfigurationService routesConfigurationService,
    ConcurrentDictionary<EsbRoute, IAdapterDi> adapterDictionary,
    ILogger<BusConsumer<TMessage, TResponse>> logger)
    : IBusConsumer<TMessage, TResponse>
    where TMessage : class
{
    //ToDO Validate Config to check  
    private EsbRoute? EsbRoute { get; set; }

    public async Task Consume(ConsumeContext<TMessage> context)
    {
        logger.LogInformation("{@Message} Message Received at {@DateTimeUtc}",
            context.Message,
            DateTime.UtcNow);
        
        
        if (routesConfigurationService.RoutesConfiguration.Routes != null && EsbRoute is null)
            foreach (var route in routesConfigurationService.RoutesConfiguration.Routes.Where(route => route.ReceiveLocation?.MessageEndpoint is not null))
            {
                if(route.ReceiveLocation?.MessageEndpoint?.EventType is null)
                        throw new MissingConfigurationException("Event Type Empty/Missing", route.Id);
                if (route.ReceiveLocation.MessageEndpoint.EventType.Equals(context.Message.GetType().GetTypeName()))
                {
                    EsbRoute = route;
                }
            }
        if (EsbRoute is null) throw new MajorConfigurationException("No Proper Route is defined");

        if (EsbRoute?.ReceiveLocation?.MessageEndpoint?.Assembly is null) throw new MissingConfigurationException("Assembly Field Empty/Missing", EsbRoute?.Id);
         var currentAssembly = Assembly.Load(EsbRoute.ReceiveLocation.MessageEndpoint.Assembly);
         if (EsbRoute?.ReceiveLocation?.MessageEndpoint?.ResponseType is null) throw new MissingConfigurationException("Response Type Empty/Missing", EsbRoute?.Id);
        var myResponseType = currentAssembly.GetType(EsbRoute.ReceiveLocation.MessageEndpoint.ResponseType);

        if (myResponseType == null)
            throw new MajorConfigurationException("Response Type couldn't be found in the assembly provided");
        var responseInstance = Activator.CreateInstance(myResponseType);
        if (responseInstance == null)
            throw new EsbRuntimeException("Failed to create an instance of the response type.");
        
        
        var publishMethod = typeof(IPublishEndpoint)
            .GetMethods()
            .FirstOrDefault(m => m.Name == "Publish" && m.IsGenericMethod);
        if (publishMethod == null)
            throw new EsbRuntimeException("Failed to find the Publish method.");
        var genericPublishMethod = publishMethod.MakeGenericMethod(myResponseType);
        if (publishMethod == null) 
            throw new EsbRuntimeException("Failed to find the Publish method for the specified type.");
        
        adapterDictionary.TryGetValue(EsbRoute, out var adapter); 
        if (adapter is HttpAdapter httpAdapter)
        {
         var jsonContent = await httpAdapter.HandleIncomingRequest(new DefaultHttpContext());
         JsonConvert.PopulateObject(jsonContent, responseInstance);
         await (Task)(genericPublishMethod.Invoke(publishEndpoint, new[] { responseInstance, new CancellationToken() })!); 
        }
        
        await Task.CompletedTask;
    }
}