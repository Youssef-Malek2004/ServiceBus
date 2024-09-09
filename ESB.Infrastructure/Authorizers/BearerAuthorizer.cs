using System.Net.Http.Headers;
using System.Net.Http.Json;
using ESB.Application.Interfaces;
using ESB.Domain.Entities.Routes;
using ESB.ErrorHandling.CustomExceptions;
using ESB.Infrastructure.Clients;

namespace ESB.Infrastructure.Authorizers;

public class BearerAuthorizer(BearerTokenCredentials bearerTokenAuthorization) : IAuthorizer<HttpClient>
{
    public async Task AuthenticateAsync(HttpClient client)
    {

        if (string.IsNullOrEmpty(bearerTokenAuthorization.Token))
        {
            // // Call login endpoint to get the token
            // if (bearerTokenAuthorization.LoginUrl != null)
            // {
            //     var token = await FetchBearerToken(bearerTokenAuthorization.LoginUrl, client);
            //     client.DefaultRequestHeaders.Authorization = 
            //         new AuthenticationHeaderValue("Bearer", token);
            // }
            throw new EsbRuntimeException("No Token Specified");

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
        var result = await client.PostAsync(loginUrl, new StringContent("")); 
        var content = await result.Content.ReadFromJsonAsync<string>();
        // Parse and return the token from response (adjust according to your API)
        return content;
    }
}