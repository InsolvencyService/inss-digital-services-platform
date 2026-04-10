using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain.Types;

public sealed class Email : TypeBase
{
    [Required(ErrorMessage = "Enter an email address")]
    [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    public string Value { get; init; }
    
    public static implicit operator string(Email email) => email.Value;

    public static implicit operator Email(string value) => new() { Value = value };
}