using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.Platform.Broker.Options;

public sealed class OneLoginIdentityProviderOptions : IdentityProviderOptions
{
    [Required]
    public string JwtPrivateKey { get; init; }
}