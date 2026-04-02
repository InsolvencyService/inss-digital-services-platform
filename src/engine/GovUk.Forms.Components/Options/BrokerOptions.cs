using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GovUk.Forms.Components.Options;

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
}