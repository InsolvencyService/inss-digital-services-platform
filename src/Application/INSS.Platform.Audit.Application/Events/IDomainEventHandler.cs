using INSS.Platform.Events.Domain;

namespace INSS.Platform.Audit.Application.Events;

/// <summary>
/// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
/// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
/// In a properly defined application the events would be documented and also adhere to a defined contract.
/// </summary>
/// <remarks>
/// Defines a handler for domain events of type <typeparamref name="TEvent"/>.
/// </remarks>
/// <typeparam name="TEvent">
/// The type of domain event to handle. Must implement <see cref="IDomainEvent"/>.
/// </typeparam>
public interface IDomainEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    /// <summary>
    /// Handles the specified domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event instance to handle.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken);
}
