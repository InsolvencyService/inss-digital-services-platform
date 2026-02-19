namespace INSS.Platform.Audit.Application.Users.Commands;

/// <summary>
/// Command to add user details to the audit platform.
/// </summary>
public class AddUserDetailsCommand
{
    /// <summary>
    /// Gets the correlation identifier for tracking the command.
    /// </summary>
    public Guid CorrelationId { get; init; }

    /// <summary>
    /// Gets the username or user identifier.
    /// </summary>
    public string User { get; init; }

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