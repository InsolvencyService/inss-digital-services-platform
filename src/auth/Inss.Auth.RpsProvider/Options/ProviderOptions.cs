using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.Auth.RpsProvider.Options;

public sealed class ProviderOptions
{
    [Required]
    public string ClientId { get; init; }

    public string[] AllowedPostLogoutRedirects { get; init; } = [];

    public bool PostLogoutRedirectAllowed(string postLogoutRedirectUri)
    {
        return AllowedPostLogoutRedirects.Any(r => r.StartsWith(postLogoutRedirectUri, StringComparison.OrdinalIgnoreCase));
    }
}