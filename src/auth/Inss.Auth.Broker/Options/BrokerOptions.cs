using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.Auth.Broker.Options;

public sealed class BrokerOptions
{
    [Required]
    public string ClientId { get; init; }
    
    [Required]
    public string JwtPublicKey { get; init; }
    
    [Required]
    public string JwtPrivateKey { get; init; }
    
    public int TokenExpiresInMinutes { get; init; } = 30;
    
    public string[] AllowedPostLogoutRedirects { get; init; } = [];

    public bool PostLogoutRedirectAllowed(string postLogoutRedirectUri)
    {
        return AllowedPostLogoutRedirects.Any(r => r.StartsWith(postLogoutRedirectUri, StringComparison.OrdinalIgnoreCase));
    }
}