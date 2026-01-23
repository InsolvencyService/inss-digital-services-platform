using INSS.Platform.Portal.Domain.Attributes;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

/// <summary>
/// Represents a model for capturing and validating a user's date of birth.
/// </summary>
public sealed class DateOfBirthModel : IHasValue<DateOnly?>, IValidatableObject
{
    /// <summary>
    /// Gets or sets the day component of the date of birth.
    /// </summary>
    [ExcludeFromSummary]
    public int? Day { get; set; }

    /// <summary>
    /// Gets or sets the month component of the date of birth.
    /// </summary>
    [ExcludeFromSummary]
    public int? Month { get; set; }

    /// <summary>
    /// Gets or sets the year component of the date of birth.
    /// </summary>
    [ExcludeFromSummary]
    public int? Year { get; set; }

    /// <summary>
    /// Gets or sets the full date of birth as a <see cref="DateOnly"/> value.
    /// </summary>
    [Required(ErrorMessage = "Enter your date of birth")]
    public DateOnly? Value
    {
        get
        {
            if (Day.HasValue && Month.HasValue && Year.HasValue)
            {
                string dateString = $"{Year.Value:D4}-{Month.Value:D2}-{Day.Value:D2}";
                if (DateOnly.TryParse(dateString, out DateOnly date))
                {
                    return date;
                }

                return null;
            }

            return null;
        }
        set
        {
            Day = value?.Day;
            Month = value?.Month;
            Year = value?.Year;
        }
    }

    /// <summary>
    /// Validates the date of birth value.
    /// </summary>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>
    /// A collection of <see cref="ValidationResult"/> objects that describe any validation failures.
    /// </returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Value is null)
        {
            yield return new ValidationResult(
                "Enter a valid date of birth",
                [nameof(Value)]);
            yield break;
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (Value >= today)
        {
            yield return new ValidationResult(
                "Date of birth must be in the past",
                [nameof(Value)]);
        }
    }
}