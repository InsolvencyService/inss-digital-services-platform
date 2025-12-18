namespace INSS.Platform.Canonical.Domain;

/// <summary>
/// Represents a base entity that provides common properties for entity identification and auditing.
/// </summary>
/// <remarks>
/// This class is intended to be used as a base type for entities that require unique identification and
/// tracking of creation and modification metadata. Derived classes can extend this type to include additional
/// domain-specific properties.
/// </remarks>
public class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the form set instance.
    /// </summary>
    public Guid InstanceId { get; set; }

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
