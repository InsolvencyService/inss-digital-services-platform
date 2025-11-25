namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents the association between an authentication policy and an authentication provider.
/// </summary>
public class AuthenticationPolicyProvider : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the authentication policy.
    /// </summary>
    public Guid AuthenticationPolicyId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the authentication provider.
    /// </summary>
    public Guid AuthenticationProviderId { get; set; }
}
