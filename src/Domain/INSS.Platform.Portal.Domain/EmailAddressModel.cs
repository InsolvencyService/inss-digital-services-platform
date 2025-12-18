using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public sealed class EmailAddressModel : BaseModel
{
    public EmailAddressModel()
    {
        PathName = "email-address";
        Title = "Email Address";
    }

    [Required(ErrorMessage = "Enter your email address")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    public string EmailAddress { get; init; } = string.Empty;
}