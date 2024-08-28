using ESB.Core.Middlewares;
using ESB.Infrastructure;
using ESB.Infrastructure.Factories;
using ESB.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfigurationService(); // InitializeConfiguration into Adding RoutesConfigurationService to be able to access in during Runtime

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient(); // Remove and add both to Separate Service
builder.Services.AddSingleton<ClientFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())    
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
var routesConfigurationService = app.Services.GetRequiredService<RoutesConfigurationService>();
app.UseEndpointMapping(routesConfigurationService);

app.MapGet("/", () => "Hello World!");

app.Run();