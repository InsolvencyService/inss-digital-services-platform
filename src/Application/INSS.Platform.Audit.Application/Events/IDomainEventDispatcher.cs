using INSS.Platform.Events.Domain;

namespace INSS.Platform.Audit.Application.Events;

/// <summary>
/// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
/// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
/// In a properly defined application the events would be documented and also adhere to a defined contract.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken);
}
