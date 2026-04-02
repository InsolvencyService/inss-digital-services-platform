using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.Auth.RpsProvider.Options;

public sealed class ProviderOptions
{
    [Required]
    public string ClientId { get; init; }
}