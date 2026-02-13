using INSS.Platform.Events.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace INSS.Platform.Audit.Application.Events;

/// <summary>
/// Dispatches domain events to their corresponding handlers.
/// </summary>
public class DomainEventDispatcher
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
    /// <param name="ct">A cancellation token to observe while waiting for the tasks to complete.</param>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
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
                    .Invoke(handler, [evt, ct])!;
                await task;
            }
        }
    }
}
