namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents a group entity within the user management domain.
/// </summary>
public class Group : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the party associated with the group.
    /// </summary>
    public Guid PartyId { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the group.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the associated <see cref="Party"/> entity for the group.
    /// </summary>
    public virtual Party? Party { get; set; }
}
