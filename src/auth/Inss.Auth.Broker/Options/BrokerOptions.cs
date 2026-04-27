using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.Auth.Broker.Options;

public sealed class BrokerOptions
{
    [Required]
    public string ClientId { get; init; }
    
    [Required]
    public string JwtPrivateKey { get; init; }

    public int TokenExpiresInMinutes { get; init; } = 30;
}