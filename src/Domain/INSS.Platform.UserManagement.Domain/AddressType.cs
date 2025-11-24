namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents a type of address with a name and description.
/// </summary>
public class AddressType : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the address type.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the address type.
    /// </summary>
    public string Description { get; set; }
}
