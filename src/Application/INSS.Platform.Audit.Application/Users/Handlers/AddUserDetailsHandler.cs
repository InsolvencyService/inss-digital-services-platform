using INSS.Platform.Audit.Application.Users.Commands;
using INSS.Platform.Canonical.Domain;

namespace INSS.Platform.Audit.Application.Users.Handlers;

/// <summary>
/// Handles operations related to adding user details.
/// </summary>
public class AddUserDetailsHandler
{
    /// <summary>
    /// Handles the addition of user details by invoking the <c>UserDetailsAdded</c> method on the specified <see cref="User"/>.
    /// </summary>
    /// <param name="user">The user entity to update with new details.</param>
    /// <param name="command">The command containing the user details to add.</param>
    public static void Handle(User user, AddUserDetailsCommand command)
    {
        user.UserDetailsAdded(
            command.User,
            command.CorrelationId,
            command.FullName,
            command.DateOfBirth,
            command.TelephoneNumber,
            command.EmailAddress
        );
    }
}
