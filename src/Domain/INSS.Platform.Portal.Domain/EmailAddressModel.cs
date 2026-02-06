using INSS.Platform.Portal.Domain.Abstract;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

/// <summary>
/// Represents a model for capturing and validating an email address input.
/// </summary>
public sealed class EmailAddressModel : IHasValue<string>
{
    /// <summary>
    /// Gets the email address value.
    /// </summary>
    /// <remarks>
    /// This property is required and must be a valid email address format.
    /// </remarks>
    [Required(ErrorMessage = "Enter your email address")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    [MaxLength(255, ErrorMessage = "Email address cannot exceed 255 characters")]
    public string Value { get; init; } = string.Empty;
}