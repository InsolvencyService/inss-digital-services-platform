namespace INSS.Platform.Events.Domain;


/// <summary>
/// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
/// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
/// In a properly defined application the events would be documented and also adhere to a defined contract.
/// </summary>
/// <remarks>
/// Represents a domain event that is raised when a user's income is added.
/// </remarks>
public sealed record UserIncomeAddedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserIncomeAddedEvent"/> record.
    /// </summary>
    /// <param name="actor">The user or system who performed the action.</param>
    /// <param name="aggregateRootId">The unique identifier of the aggregate root associated with the event.</param>
    /// <param name="correlationId">The unique identifier used to correlate related events.</param>
    /// <param name="incomeProvider">The name of the income provider.</param>
    /// <param name="grossIncome">The gross income amount added.</param>
    public UserIncomeAddedEvent(string actor, Guid aggregateRootId, Guid correlationId, string incomeProvider, decimal grossIncome)
        : base(actor, aggregateRootId, correlationId)
    {
        IncomeProvider = incomeProvider;
        GrossIncome = grossIncome;
    }
    
    /// <summary>
    /// Gets the name of the income provider.
    /// </summary>
    public string IncomeProvider { get; init; }

    /// <summary>
    /// Gets the gross income amount that was added.
    /// </summary>
    public decimal GrossIncome { get; init; }
}
