namespace INSS.Platform.Events.Domain;

/// <summary>
/// Represents a domain event with actor, correlation, and aggregate root information.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEvent"/> class.
    /// </summary>
    /// <param name="actor">The actor responsible for the event.</param>
    /// <param name="aggregateRootId">Optional. The unique identifier of the aggregate root.</param>
    /// <param name="correlationId">Optional. The unique identifier for correlating events.</param>
    public DomainEvent(string actor, Guid? aggregateRootId = null, Guid? correlationId = null)
    {
        Actor = actor;
        CorrelationId = correlationId ?? Guid.NewGuid();
        AggregateRootId = aggregateRootId ?? Guid.NewGuid();
    }

    /// <inheritdoc />
    public Guid EventId => Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredOnUtc => DateTime.UtcNow;

    /// <inheritdoc />
    public string Actor { get; init; }

    /// <inheritdoc />
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public Guid AggregateRootId { get; init; } = Guid.NewGuid();
}
