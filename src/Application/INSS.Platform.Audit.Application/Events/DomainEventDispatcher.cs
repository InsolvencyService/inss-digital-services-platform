using INSS.Platform.Events.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace INSS.Platform.Audit.Application.Events;

/// <summary>
/// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
/// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
/// In a properly defined application the events would be documented and also adhere to a defined contract.
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventDispatcher"/> class.
    /// </summary>
    /// <param name="services">The service provider used to resolve event handlers.</param>
    public DomainEventDispatcher(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Dispatches the specified domain events asynchronously to their registered handlers.
    /// </summary>
    /// <param name="events">The collection of domain events to dispatch.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the tasks to complete.</param>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken)
    {
        foreach (IDomainEvent evt in events)
        {
            Type eventType = evt.GetType();
            Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
            IEnumerable<object?> handlers = _services.GetServices(handlerType);
            foreach (object? handler in handlers)
            {
                Task task = (Task)handlerType
                    .GetMethod("Handle")!
                    .Invoke(handler, [evt, cancellationToken])!;

                await task;
            }
        }
    }
}
