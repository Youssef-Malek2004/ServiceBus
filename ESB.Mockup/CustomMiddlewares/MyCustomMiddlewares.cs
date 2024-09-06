using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace ESB.Mockup.CustomMiddlewares;

public static class MyCustomMiddlewares
{
    public static IEndpointRouteBuilder AddTestRoutes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/orders", [Authorize] async (HttpContext context, HttpResponse response) =>
        {
            if (context.Request.Query.TryGetValue("test", out var value))
            {
                Console.WriteLine(value[0]);   
            }
            var orders = new List<object>
            {
                new
                {
                    OrderId = 1,
                    CustomerName = "John Doe",
                    TotalAmount = 99.99,
                    OrderDate = new DateTime(2024, 8, 29)
                },
                new
                {
                    OrderId = 2,
                    CustomerName = "Jane Smith",
                    TotalAmount = 149.50,
                    OrderDate = new DateTime(2024, 8, 30)
                },
                new
                {
                    OrderId = 3,
                    CustomerName = "Alice Johnson",
                    TotalAmount = 200.00,
                    OrderDate = new DateTime(2024, 8, 28)
                }
            };

            await context.Response.WriteAsJsonAsync(orders);
        });

        return app;
    }

    public static IEndpointRouteBuilder AddAuthorizationRoutes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/testBasicAuth", async (HttpContext context, HttpResponse response) =>
        {
            // Create the Basic Authentication header value
            var username = "user";
            var password = "password";
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            using var client = new HttpClient();
    
            // Set the Authorization header with the correct format
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

            // Make the request to the specified URL
            var forwardedResponse = await client.GetAsync("http://localhost:5172/orders");
    
            // Set the response status code and content type based on the forwarded response
            context.Response.StatusCode = (int)forwardedResponse.StatusCode;
            context.Response.ContentType = forwardedResponse.Content.Headers.ContentType?.ToString();

            // Read and write the response content
            var responseContent = await forwardedResponse.Content.ReadAsStringAsync();
            await context.Response.WriteAsync(responseContent);
        });
        return app;
    }
}