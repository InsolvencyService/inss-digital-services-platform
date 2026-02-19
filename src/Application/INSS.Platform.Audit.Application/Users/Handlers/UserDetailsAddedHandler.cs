using INSS.Platform.Audit.Application.Events;
using INSS.Platform.Audit.Domain;
using INSS.Platform.Events.Domain;

namespace INSS.Platform.Audit.Application.Users.Handlers;

/// <summary>
/// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
/// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
/// In a properly defined application the events would be documented and also adhere to a defined contract.
/// </summary>
public class UserDetailsAddedHandler :  IDomainEventHandler<UserDetailsAddedEvent>
{
    private readonly IAuditService _auditService;

    public UserDetailsAddedHandler(IAuditService auditService)
    {
        _auditService = auditService;
    }

    /// <summary>
    /// Handles the <see cref="UserDetailsAddedEvent"/> by creating an audit entry and recording it using the <see cref="IAuditService"/>.
    /// </summary>
    /// <param name="domainEvent">The event containing details of the user whose details were added.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(UserDetailsAddedEvent domainEvent, CancellationToken cancellationToken)
    {
        AuditEntry auditEntry = new(
            EventType: nameof(UserDetailsAddedEvent),
            TimestampUtc: DateTime.UtcNow,
            Description: $"User details added: {domainEvent.FullName}",
            Metadata: domainEvent
        );
        
        await _auditService.RecordAsync(auditEntry, cancellationToken);
    }
}