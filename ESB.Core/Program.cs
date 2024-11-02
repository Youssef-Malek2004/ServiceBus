using ESB.Application.Routes;
using ESB.Core.Middlewares;
using ESB.Infrastructure;
using ESB.Infrastructure.ExceptionHandlers;
using ESB.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration)));

builder.Services.AddConfigurationService(builder.Configuration); 

builder.Services.AddRequiredComponents();
builder.Services.ConfigureBus(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())    
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();

app.AddAdapters();
app.AddResiliency();
app.AddHttpReceiveEndpoints();

app.MapGet("configTests", (IOptionsSnapshot<ConfiguredRoutes> routesConfigurationService) =>
{
    return Results.Ok(routesConfigurationService.Value.Routes[0].Id);
});


app.Run();
