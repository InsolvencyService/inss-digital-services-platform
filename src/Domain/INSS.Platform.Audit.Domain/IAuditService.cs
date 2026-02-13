namespace INSS.Platform.Audit.Domain;

/// <summary>
/// Defines the contract for recording audit entries asynchronously.
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
