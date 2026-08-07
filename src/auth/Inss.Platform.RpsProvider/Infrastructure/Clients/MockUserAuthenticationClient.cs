using Inss.Platform.RpsProvider.Application.Clients;
using Inss.Platform.RpsProvider.Domain.Enums;

namespace Inss.Platform.RpsProvider.Infrastructure.Clients;

public sealed class MockUserAuthenticationClient : IUserAuthenticationClient
{
    public async Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password, string csrfToken)
    {
        await Task.Delay(10);
        Console.WriteLine("Calling login...");
        
        // Test cases...
        return email switch
        {
            "invalid@temp.org" => RpsAuthenticationTypes.Unknown,
            "locked@temp.org" => RpsAuthenticationTypes.Locked,
            _ => RpsAuthenticationTypes.Matched
        };
    }
}