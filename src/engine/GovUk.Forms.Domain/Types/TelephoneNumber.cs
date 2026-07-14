using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain.Types;

public sealed class TelephoneNumber : TypeBase
{
    [Phone(ErrorMessage = "Enter a valid telephone number")]
    [Required(ErrorMessage = "Enter your telephone number")]
    [RegularExpression(@"^[+0-9\s\-()]{6,20}$", ErrorMessage = "Telephone number must be between 6 and 20 characters and can only contain numbers, spaces, hyphens, brackets and an optional + for country code")]
    public string Value { get; set; }

    public string Label { get; init; } = "Telephone number";

    public string? Hint { get; init; } = "Enter your telephone number";
    
    public LabelSizes LabelSize { get; init; } = LabelSizes.Small;
}