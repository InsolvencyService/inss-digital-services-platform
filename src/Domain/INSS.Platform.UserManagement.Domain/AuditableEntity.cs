namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents an entity with audit information such as creation and modification details.
/// </summary>
public class AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTime? Created { get; set; }

    /// <summary>
    /// Gets or sets the username or identifier of the user who created the entity.
    /// </summary>
    public string? CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    public DateTime? Modified { get; set; }

    /// <summary>
    /// Gets or sets the username or identifier of the user who last modified the entity.
    /// </summary>
    public string? ModifiedBy { get; set; }
}
