using System.ComponentModel.DataAnnotations;

namespace Inss.Auth.RpsProvider.Options;

public sealed class ProviderOptions
{
    [Required]
    public string ClientId { get; init; }
}