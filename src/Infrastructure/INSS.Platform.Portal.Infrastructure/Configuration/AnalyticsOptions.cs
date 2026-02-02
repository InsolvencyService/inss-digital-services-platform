using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Infrastructure.Configuration;

/// <summary>
/// Represents configuration options for analytics integration.
/// </summary>
public class AnalyticsOptions
{
    /// <summary>
    /// Gets or sets the base URL for the analytics service.
    /// </summary>
    [Required]
    public string BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the site identifier used by the analytics service.
    /// </summary>
    [Required]
    public string SiteId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether analytics is enabled.
    /// </summary>
    [Required]
    public bool Enabled { get; set; }
}
