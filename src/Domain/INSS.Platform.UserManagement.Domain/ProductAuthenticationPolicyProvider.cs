namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents the association between a product and its authentication policy provider.
/// </summary>
public class ProductAuthenticationPolicyProvider : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the product.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the authentication policy provider.
    /// </summary>
    public Guid AuthenticationPolicyProviderId { get; set; }
}
