namespace INSS.Platform.Audit.Application.Users.Commands;

/// <summary>
/// Command to add income information for a user.
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
