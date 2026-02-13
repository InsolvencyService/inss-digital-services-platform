namespace INSS.Platform.Events.Domain;

/// <summary>
/// Represents a domain event that is raised when a user's income is added.
/// </summary>
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
