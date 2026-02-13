using INSS.Platform.Audit.Application.Users.Commands;
using INSS.Platform.Canonical.Domain;

namespace INSS.Platform.Audit.Application.Users.Handlers;

/// <summary>
/// Handler responsible for processing the addition of user income.
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
