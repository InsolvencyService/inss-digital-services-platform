namespace INSS.Platform.Events.Domain;

/// <summary>
/// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
/// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
/// In a properly defined application the events would be documented and also adhere to a defined contract.
/// </summary>
public sealed record UserDetailsAddedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserDetailsAddedEvent"/> record.
    /// </summary>
    /// <param name="actor">The user or system who performed the action.</param>
    /// <param name="aggregateRootId">The unique identifier of the aggregate root.</param>
    /// <param name="correlationId">The unique identifier used to correlate related events.</param>
    /// <param name="fullName">The full name of the user.</param>
    /// <param name="dateOfBirth">The date of birth of the user.</param>
    /// <param name="telephoneNumber">The telephone number of the user.</param>
    /// <param name="emailAddress">The email address of the user.</param>
    public UserDetailsAddedEvent(
        string actor,
        Guid aggregateRootId,
        Guid correlationId,
        string fullName,
        DateOnly dateOfBirth,
        string telephoneNumber,
        string emailAddress)
        : base(actor, aggregateRootId, correlationId)
    {
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        TelephoneNumber = telephoneNumber;
        EmailAddress = emailAddress;
    }

    /// <summary>
    /// Gets the full name of the user.
    /// </summary>
    public string FullName { get; init; }

    /// <summary>
    /// Gets the date of birth of the user.
    /// </summary>
    public DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Gets the telephone number of the user.
    /// </summary>
    public string TelephoneNumber { get; init; }
    
    /// <summary>
    /// Gets the email address of the user.
    /// </summary>
    public string EmailAddress { get; init; }
}
