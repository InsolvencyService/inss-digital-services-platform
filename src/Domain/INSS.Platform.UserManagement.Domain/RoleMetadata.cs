namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents metadata associated with a user role.
/// </summary>
public class RoleMetadata : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the associated role.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the name of the metadata.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the value of the metadata.
    /// </summary>
    public string Value { get; set; }
}
