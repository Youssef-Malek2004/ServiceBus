namespace ESB.Infrastructure.ExceptionHandlers;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Call the next middleware in the pipeline
            await next(context);
        }
        catch (Exception ex)
        {
            await ExceptionHandlers.Handle(ex, logger);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log the exception
        logger.LogError(exception, "An unhandled exception occurred");

        // Set the response details
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Customize the error response as needed
        var response = new
        {
            error = "An unexpected error occurred.",
            details = exception.Message // You can customize this to show more details
        };

        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
}