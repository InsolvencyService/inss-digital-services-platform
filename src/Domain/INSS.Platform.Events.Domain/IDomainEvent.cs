namespace INSS.Platform.Events.Domain;

/// <summary>
/// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
/// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
/// In a properly defined application the events would be documented and also adhere to a defined contract.
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
