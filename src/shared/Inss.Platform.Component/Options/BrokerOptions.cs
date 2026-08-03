using System.ComponentModel.DataAnnotations;

namespace Inss.Platform.Component.Options;

public class BrokerOptions
{
    [Required]
    public string Authority { get; init; }
    
    [Required]
    public string ClientId { get; init; }
    
    [Required]
    public string JwtPublicKey { get; init; }

    public string[] Scopes { get; init; } = [];
    
    [Required]
    public string LogoutRedirectUrl { get; init; }
    
    public IdentityProviderTypes? IdentityProvider { get; init; }
    
    public enum IdentityProviderTypes
    {
        Rps,
        Entra,
        OneLogin
    }
}