namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents a user role within the system.
/// </summary>
public class Role : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the role.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
