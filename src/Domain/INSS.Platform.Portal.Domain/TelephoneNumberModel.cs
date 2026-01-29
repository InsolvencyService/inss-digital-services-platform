using INSS.Platform.Portal.Domain.Abstract;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

/// <summary>
/// Represents a model for capturing and validating a telephone number input.
/// </summary>
public sealed class TelephoneNumberModel : IHasValue<string>
{
    /// <summary>
    /// Gets or sets the telephone number value.
    /// </summary>
    /// <remarks>
    /// The value must be between 6 and 20 characters and can only contain numbers, spaces, hyphens, brackets, and an optional plus sign for the country code.
    /// </remarks>
    [Phone(ErrorMessage = "Enter a valid telephone number")]
    [Required(ErrorMessage = "Enter your telephone number")]
    [RegularExpression(@"^[+0-9\s\-()]{6,20}$", ErrorMessage = "Telephone number must be between 6 and 20 characters and can only contain numbers, spaces, hyphens, brackets and an optional + for country code")]
    public string Value { get; set; } = string.Empty;
}