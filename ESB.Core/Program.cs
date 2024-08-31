using ESB.Core.Middlewares;
using ESB.Infrastructure;
using ESB.Messages.Events;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// InitializeConfiguration into Adding RoutesConfigurationService to be able to access in during Runtime
builder.Services.AddConfigurationService(); 
// Adds HttpFactory, ClientFactory, Dictionaries
builder.Services.AddRequiredComponents();
//Adds MassTransit Configuration
builder.Services.ConfigureBus(builder.Configuration);
//Swagger EndpointsExplorer and Generator
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())    
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
//Adds all Adapters to the dictionary where EsbRoute -> Proper Adapter configured for that send location
app.AddAdapters(); 
//Adds all the HttpReceiveEndpoints -> Once ftp are needed merge with a proper Middleware for addingAllEndpoints
app.AddHttpReceiveEndpoints();

app.MapGet("/", async (IPublishEndpoint publishEndpoint, HttpContext context) =>
{
    await publishEndpoint.Publish(new TestEvent
    {
        Num = 5
    });
    await context.Response.WriteAsJsonAsync("hi");
});
app.MapGet("/route", async (IPublishEndpoint publishEndpoint, HttpContext context) =>
{
    await context.Response.WriteAsJsonAsync(new
    {
        Num = 5
    });
});

app.Run();