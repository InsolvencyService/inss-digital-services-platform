namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents the association between a party and a product role.
/// </summary>
public class PartyProductRole : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the party.
    /// </summary>
    public Guid PartyId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the product role.
    /// </summary>
    public Guid ProductRoleId { get; set; }
}
