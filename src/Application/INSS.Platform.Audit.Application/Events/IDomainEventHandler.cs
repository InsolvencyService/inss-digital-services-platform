using INSS.Platform.Events.Domain;

namespace INSS.Platform.Audit.Application.Events;

/// <summary>
/// Defines a handler for domain events of type <typeparamref name="TEvent"/>.
/// </summary>
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
