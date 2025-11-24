using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Auth.API.Models;

/// <summary>
/// Represents configuration options for Azure Entra authentication.
/// </summary>
public class EntraOptions
{
    /// <summary>
    /// Gets or sets the client ID for the Azure Entra application.
    /// </summary>
    [Required]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret for the Azure Entra application.
    /// </summary>
    [Required]
    public string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the base URI for the Azure Entra endpoint.
    /// </summary>
    [Required]
    public string BaseUri { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier for the Azure Entra directory.
    /// </summary>
    [Required]
    public string Tenant { get; set; }

    /// <summary>
    /// Gets or sets the callback path used during the sign-in process with Entra.
    /// </summary>
    [Required]
    public string SignInCallbackPath { get; set; }

    /// <summary>
    /// Gets or sets the callback path used during the sign-out process with Entra.
    /// </summary>
    [Required]
    public string SignOutCallbackPath { get; set; }

    /// <summary>
    /// Gets or sets the list of scopes requested during authentication.
    /// </summary>
    [Required]
    public List<string> Scopes { get; set; } = new();
}
