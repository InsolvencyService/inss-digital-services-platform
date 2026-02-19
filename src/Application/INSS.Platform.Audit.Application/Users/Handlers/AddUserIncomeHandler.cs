using INSS.Platform.Audit.Application.Users.Commands;
using INSS.Platform.Canonical.Domain;

namespace INSS.Platform.Audit.Application.Users.Handlers;

/// <summary>
/// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
/// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
/// In a properly defined application the events would be documented and also adhere to a defined contract.
/// </summary>
public class AddUserIncomeHandler
{
    /// <summary>
    /// Handles the addition of income information to a user.
    /// </summary>
    /// <param name="user">The user entity to which income will be added.</param>
    /// <param name="command">The command containing income details.</param>
    public static void Handle(User user, AddUserIncomeCommand command)
    {
        user.UserIncomeAdded(command.User, command.CorrelationId, command.GrossIncome, command.IncomeProvider);
    }
}
