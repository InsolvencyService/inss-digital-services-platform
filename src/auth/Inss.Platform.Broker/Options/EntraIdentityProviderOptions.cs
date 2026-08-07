using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.Platform.Broker.Options;

public sealed class EntraIdentityProviderOptions : IdentityProviderOptions
{
    [Required]
    public string ClientSecret { get; init; }
}