using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public sealed class TelephoneNumberModel : BaseModel
{
    public TelephoneNumberModel()
    {
        PathName = "telephone-number";
        Name = "UK phone number";
    }

    [Phone(ErrorMessage = "Enter a valid telephone number")]
    [Required(ErrorMessage = "Enter your telephone number")]
    [RegularExpression(@"^[+0-9\s\-()]{6,20}$", ErrorMessage = "Telephone number must be between 6 and 20 characters and can only contain numbers, spaces, hyphens, brackets and an optional + for country code")]
    public string TelephoneNumber { get; init; } = string.Empty;
}