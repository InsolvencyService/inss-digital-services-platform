using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Application.Options;

/// <summary>
/// Represents configuration options for the Bank validation API client.
/// </summary>
public class BankValidationOptions
{
    /// <summary>
    /// Gets or sets the base URL of the Bank validation API.
    /// </summary>
    [Required]
    public string BaseApiUrl { get; set;  }

    /// <summary>
    /// Gets or sets the API key used to authenticate with the Bank validation API.
    /// </summary>
    [Required]
    public string ApiKey { get; set; }
}
