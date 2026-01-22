using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace INSS.Platform.Portal.Domain.Validation;

/// <summary>
/// Validation attribute to validate a building society roll number.
/// Ensures the value is between 1 and 18 characters and contains only allowed characters:
/// letters (a-z, A-Z), numbers (0-9), hyphens, spaces, forward slashes, and full stops.
/// </summary>
public partial class BuildingSocietyRollNumberAttribute : ValidationAttribute
{
    /// <summary>
    /// The compiled regular expression used to validate the roll number format.
    /// </summary>
    private static readonly Regex _regex = BuildingSocietyRollNumberRegex();

    /// <summary>
    /// Validates the specified value with respect to the current validation attribute.
    /// </summary>
    /// <param name="value">The value of the object to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>
    /// A <see cref="ValidationResult"/> indicating whether validation succeeded.
    /// </returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        string? rollNumber = value as string;

        if (string.IsNullOrEmpty(rollNumber))
        {
            return ValidationResult.Success;
        }

        if (rollNumber.Length is < 1 or > 18)
        {
            return new ValidationResult("Building society roll number must be between 1 and 18 characters");
        }

        if (!_regex.IsMatch(rollNumber))
        {
            return new ValidationResult("Building society roll number must only include letters a to z, numbers, hyphens, spaces, forward slashes and full stops");
        }

        return ValidationResult.Success;
    }

    /// <summary>
    /// Returns a compiled regular expression that matches valid building society roll numbers.
    /// Allowed characters: letters (a-z, A-Z), numbers (0-9), hyphens, spaces, forward slashes, and full stops.
    /// </summary>
    /// <returns>A compiled <see cref="Regex"/> instance for roll number validation.</returns>
    [GeneratedRegex(@"^[A-Za-z0-9\-\/\. ]+$", RegexOptions.Compiled)]
    private static partial Regex BuildingSocietyRollNumberRegex();
}