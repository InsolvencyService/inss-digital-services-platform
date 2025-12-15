using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public sealed class EmailAddressModel : BaseModel
{
    public EmailAddressModel()
    {
        PathName = "email-address";
        Name = "Email Address";
    }

    [Required(ErrorMessage = "Enter your email address")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Enter a valid email address")]
    public string EmailAddress { get; init; } = string.Empty;
}