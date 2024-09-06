using System.Net.Http.Headers;
using System.Text;
using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;

namespace ESB.Infrastructure.Authorizers;

public class BasicAuthorizer(BasicAuthorization basicAuthorization) : IAuthorizer<HttpClient>
{
    public Task AuthenticateAsync(HttpClient client)
    {
        var byteArray = Encoding.ASCII.GetBytes($"{basicAuthorization.Username}:{basicAuthorization.Password}");
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        return Task.CompletedTask;
    }
}