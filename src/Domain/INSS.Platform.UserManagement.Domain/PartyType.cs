namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents a type of party within the user management system.
/// </summary>
public class PartyType : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the party type.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the party type.
    /// </summary>
    public string Description { get; set; }
}
