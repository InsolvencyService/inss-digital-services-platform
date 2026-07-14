using System.ComponentModel.DataAnnotations;
using System.Text.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.Auth.RpsProvider.Options;

public sealed class ProviderOptions
{
    [Required]
    public string ClientId { get; init; }

    public string AllowedPostLogoutRedirects { get; init; }

    public bool PostLogoutRedirectAllowed(string postLogoutRedirectUri)
    {
        string[] allowedList = JsonSerializer.Deserialize<string[]>(AllowedPostLogoutRedirects) ?? [];
        return allowedList.Any(r => r.StartsWith(postLogoutRedirectUri, StringComparison.OrdinalIgnoreCase));
    }
}