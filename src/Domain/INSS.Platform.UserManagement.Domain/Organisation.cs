namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents an organisation entity within the user management domain.
/// </summary>
public class Organisation : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the party associated with the organisation.
    /// </summary>
    public Guid PartyId { get; set; }

    /// <summary>
    /// Gets or sets the name of the organisation.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the organisation.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the optional company identifier for the organisation.
    /// </summary>
    public string? CompanyIdentifier { get; set; }

    /// <summary>
    /// Gets or sets the associated <see cref="Party"/> entity for the Organisation.
    /// </summary>
    public virtual Party? Party { get; set; }
}
