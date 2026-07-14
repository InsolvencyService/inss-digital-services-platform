using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain.Types;

public sealed class EmailAddress : TypeBase
{
    [Required(ErrorMessage = "Enter an email address")]
    [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    public string Value { get; set; }
    
    public string Label { get; init; } = "Email address";

    public string? Hint { get; init; } = "Enter your email address";

    public LabelSizes LabelSize { get; init; } = LabelSizes.Small;
}