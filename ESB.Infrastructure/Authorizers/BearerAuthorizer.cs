using System.Net.Http.Headers;
using System.Net.Http.Json;
using ESB.Configurations.Interfaces;
using ESB.Configurations.Routes;
using ESB.Infrastructure.Clients;

namespace ESB.Infrastructure.Authorizers;

public class BearerAuthorizer(BearerTokenAuthorization bearerTokenAuthorization) : IAuthorizer<HttpClient>
{
    public async Task AuthenticateAsync(HttpClient client)
    {

        if (string.IsNullOrEmpty(bearerTokenAuthorization.Token))
        {
            // Call login endpoint to get the token
            if (bearerTokenAuthorization.LoginUrl != null)
            {
                var token = await FetchBearerToken(bearerTokenAuthorization.LoginUrl, client);
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
            }

            // You may also implement token refresh logic based on bearerAuth.RefreshTokenInterval
        }
        else
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerTokenAuthorization.Token);
        }
    }
    private async Task<string?> FetchBearerToken(string loginUrl, HttpClient client)
    {
        // Implement logic to fetch bearer token from login endpoint//Todo factory 
        var result = await client.PostAsync(loginUrl, new StringContent("")); 
        var content = await result.Content.ReadFromJsonAsync<string>();
        // Parse and return the token from response (adjust according to your API)
        return content;
    }
}