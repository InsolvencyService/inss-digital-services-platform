namespace INSS.Platform.Events.Domain;

/// <summary>
/// Represents a domain event with metadata for auditing and correlation.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for this event instance.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the UTC timestamp when the event occurred.
    /// </summary>
    DateTime OccurredOnUtc { get; }

    /// <summary>
    /// Gets the identifier of the actor e.g.User or System that triggered the event.
    /// </summary>
    string Actor { get; init; }

    /// <summary>
    /// Gets or sets the correlation identifier for linking related events.
    /// </summary>
    Guid CorrelationId { get; init; }

    /// <summary>
    /// Gets the unique identifier for the aggregate root associated with this entity.  
    /// </summary>
    /// <remarks>This identifier is essential for tracking the state and behavior of the aggregate root within
    /// the domain model.</remarks>
    Guid AggregateRootId { get; init; }
}
