namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents metadata information for an authentication provider.
/// </summary>
public class AuthenticationProviderMetadata : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the authentication provider.
    /// </summary>
    public Guid AuthenticationProviderId { get; set; }

    /// <summary>
    /// Gets or sets the client identifier used for authentication.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the secret associated with the authentication provider.
    /// </summary>
    public string? Secret { get; set; }

    /// <summary>
    /// Gets or sets the authorization endpoint URL.
    /// </summary>
    public string? AuthorizeEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the token endpoint URL.
    /// </summary>
    public string? TokenEndpoint { get; set; }
}
