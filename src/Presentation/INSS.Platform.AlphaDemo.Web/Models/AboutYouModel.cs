using INSS.Platform.Portal.Domain.Forms;

namespace INSS.Platform.AlphaDemo.Web.Models;

/// <summary>
/// Represents the model for collecting information about the user,
/// including full name, telephone number, email address, address, and date of birth.
/// </summary>
public sealed class AboutYouModel : FormBase
{
    /// <summary>
    /// Gets or sets the user's full name.
    /// </summary>
    public UserFullNameModel FullName { get; set; }

    /// <summary>
    /// Gets or sets the user's telephone number.
    /// </summary>
    public TelephoneNumberModel TelephoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public EmailAddressModel EmailAddress { get; set; }

    /// <summary>
    /// Gets or sets the user's address.
    /// </summary>
    public UserAddressModel Address { get; set; } = new UserAddressModel();

    /// <summary>
    /// Gets or sets the user's date of birth.
    /// </summary>
    public DateOfBirthModel DateOfBirth { get; set; } = new DateOfBirthModel();
}
