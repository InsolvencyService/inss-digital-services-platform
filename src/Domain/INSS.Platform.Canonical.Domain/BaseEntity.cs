using INSS.Platform.Events.Domain;

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
    /// Backing field for the <see cref="DomainEvents"/> property. Stores domain events associated with the entity.
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets a read-only collection of domain events that have occurred for this entity.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

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

    /// <summary>
    /// Adds a domain event to the entity's domain event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes all domain events from the entity's domain event collection.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
