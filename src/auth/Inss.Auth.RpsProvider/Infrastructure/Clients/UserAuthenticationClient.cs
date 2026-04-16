using Inss.Auth.RpsProvider.Application.Clients;
using Inss.Auth.RpsProvider.Domain.Enums;

namespace Inss.Auth.RpsProvider.Infrastructure.Clients;

public sealed class UserAuthenticationClient : IUserAuthenticationClient
{
    private readonly HttpClient _client;

    public UserAuthenticationClient(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password)
    {
        // TODO: This is where we will provide the connection to RPS and handle the response
        
        Console.WriteLine("Calling RPS...");
        HttpResponseMessage response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();

        // Test cases...
        return email switch
        {
            "invalid@temp.org" => RpsAuthenticationTypes.Unknown,
            "locked@temp.org" => RpsAuthenticationTypes.Locked,
            _ => RpsAuthenticationTypes.Matched
        };
    }
}