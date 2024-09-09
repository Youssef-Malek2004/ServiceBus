using ESB.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ESB.Application.Behaviors;

public class LoggingPipelineBehavior<TRequest, TResponse>(ILogger<LoggingPipelineBehavior<TRequest,TResponse>> logger) :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Starting Request {@RequestName} , {@DateTimeUtc}",
            typeof(TRequest).Name,
            DateTime.UtcNow);
        
        var result = await next();
        
        logger.LogInformation(
            "Completed Request {@RequestName} , {@DateTimeUtc}",
            typeof(TRequest).Name,
            DateTime.UtcNow);
        
        return result;
    }
}