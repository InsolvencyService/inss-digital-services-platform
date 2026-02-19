namespace INSS.Platform.Events.Domain;

/// <summary>
/// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
/// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
/// In a properly defined application the events would be documented and also adhere to a defined contract.
/// </summary>
public sealed record UserBankDetailsAddedEvent : DomainEvent
{
    public UserBankDetailsAddedEvent(string actor, Guid aggregateRootId, Guid correlationId, string accountName, string sortCode)
        : base(actor, aggregateRootId, correlationId)
    {
        AccountName = accountName;
        SortCode = sortCode;
    }
    
    public string AccountName { get; init; }

    public string SortCode { get; init; }
}
