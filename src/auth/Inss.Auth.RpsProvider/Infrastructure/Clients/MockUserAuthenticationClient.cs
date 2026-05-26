using Inss.Auth.RpsProvider.Application.Clients;
using Inss.Auth.RpsProvider.Domain.Enums;

namespace Inss.Auth.RpsProvider.Infrastructure.Clients;

public sealed class MockUserAuthenticationClient : IUserAuthenticationClient
{
    private readonly HttpClient _client;

    public MockUserAuthenticationClient(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password, string csrfToken)
    {
        Console.WriteLine("Calling login...");
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