namespace INSS.Platform.Audit.Application.Users.Commands;

/// <summary>
/// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
/// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
/// In a properly defined application the events would be documented and also adhere to a defined contract.
/// </summary>
public class AddUserIncomeCommand
{
    /// <summary>
    /// Gets the correlation identifier for tracking the command.
    /// </summary>
    public Guid CorrelationId { get; init; }

    /// <summary>
    /// Gets the user identifier or username.
    /// </summary>
    public string User { get; init; }

    /// <summary>
    /// Gets the name of the income provider.
    /// </summary>
    public string IncomeProvider { get; init; }

    /// <summary>
    /// Gets the gross income amount.
    /// </summary>
    public decimal GrossIncome { get; init; }
}
