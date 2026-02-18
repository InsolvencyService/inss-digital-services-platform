namespace INSS.Platform.Audit.Domain;

/// <summary>
/// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
/// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
/// In a properly defined application the events would be documented and also adhere to a defined contract.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Records an audit entry asynchronously.
    /// </summary>
    /// <param name="auditEntry">The audit entry to record.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RecordAsync(AuditEntry auditEntry, CancellationToken cancellationToken);
}
