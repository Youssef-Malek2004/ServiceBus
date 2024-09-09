using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ESB.Mockup.AuthenticationHandlers;
using ESB.Mockup.CustomMiddlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddAuthentication("BasicAuthentication")
//     .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourIssuer",
            ValidAudience = "yourAudience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THISISMYSECRETKEYBUDDIESdawdwadadadwawd"))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//
// app.Use(async (context, next) =>
// {
//     await next();
//
//     if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
//     {
//         context.Response.ContentType = "application/json";
//         await context.Response.WriteAsync("{\"error\": \"Authorization failed: Unauthorized access.\"}");
//     }
//     else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
//     {
//         context.Response.ContentType = "application/json";
//         await context.Response.WriteAsync("{\"error\": \"Authorization failed: Forbidden.\"}");
//     }
// });


app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.AddAuthorizationRoutes();
// app.AddTestRoutes();

app.MapPost("/login", (HttpContext context) =>
{
    // var username = context.Request.Form["username"];
    // var password = context.Request.Form["password"];

    // // Validate username and password (this is just a placeholder)
    // if (username == "user" && password == "password")
    // {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "testing")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THISISMYSECRETKEYBUDDIESdawdwadadadwawd"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "yourIssuer",
            audience: "yourAudience",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return Results.Ok(new JwtSecurityTokenHandler().WriteToken(token));
    // }
    //
    // return Results.Unauthorized();
});

app.MapGet("/orders", [Authorize] async (HttpContext context) =>
{
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

app.MapGet("/getNum", async (HttpResponse response, HttpContext context) =>
{
    await context.Response.WriteAsJsonAsync(new
    {
        Num = 5
    });
}); 

app.MapGet("/getString", [Authorize]async (HttpResponse response, HttpContext context) =>
{
    await context.Response.WriteAsJsonAsync(new
    {
        Name = "Malek"
    });
});

app.Run();
