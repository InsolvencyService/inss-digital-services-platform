using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Application.Options;

/// <summary>
/// Represents configuration options for the Canonical Data API.
/// </summary>
public class CanonicalDataOptions
{
    /// <summary>
    /// Gets or sets the base URL for the Canonical Data API.
    /// </summary>
    [Required]
    public string BaseApiUrl { get; set; } = string.Empty;
}
