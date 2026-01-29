using INSS.Platform.Portal.Domain.Abstract;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

/// <summary>
/// Represents a model for capturing a user's full name with validation.
/// </summary>
public sealed class UserFullNameModel : IHasValue<string>
{
    /// <summary>
    /// Gets the user's full name.
    /// </summary>
    /// <remarks>
    /// This value is required and must only contain letters, spaces, hyphens, and apostrophes.
    /// </remarks>
    [Required(ErrorMessage = "Enter your full name")]
    [RegularExpression("^[A-Za-z\\s\\-']+$", ErrorMessage = "Full name can only contain letters, spaces, hyphens and apostrophes")]
    public string Value { get; init; } = string.Empty;
}