namespace INSS.Platform.Audit.Domain;

/// <summary>
/// Represents an audit entry containing event details, timestamp, description, and optional metadata.
/// </summary>
/// <param name="EventType">The type of the event being audited.</param>
/// <param name="TimestampUtc">The UTC timestamp when the event occurred.</param>
/// <param name="Description">A description of the event.</param>
/// <param name="Metadata">Optional additional metadata related to the event.</param>
public record AuditEntry(
    string EventType,
    DateTime TimestampUtc,
    string Description,
    object? Metadata
);

