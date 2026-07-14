using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.Auth.Broker.Options;

public abstract class IdentityProviderOptions
{
    [Required]
    public string Authority { get; init; }
    
    [Required]
    public string ClientId { get; init; }
    
    [Required]
    public string[] Scopes { get; init; }
}