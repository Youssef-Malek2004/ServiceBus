using System.Net.Http.Headers;
using System.Text;
using ESB.Application.Interfaces;
using ESB.Application.Routes;

namespace ESB.Infrastructure.Authorizers;

public class BasicAuthorizer(BasicCredentials? basicAuthorization) : IAuthorizer<HttpClient>
{
    public Task AuthenticateAsync(HttpClient client)
    {
        var byteArray = Encoding.ASCII.GetBytes($"{basicAuthorization?.Username}:{basicAuthorization?.Password}");
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        return Task.CompletedTask;
    }
}