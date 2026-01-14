using INSS.Platform.Portal.Domain.Attributes;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain.Forms;

/// <summary>
/// Represents the view model for collecting personal information about a user.
/// This model is used to capture basic personal details in the About You form.
/// </summary>
public sealed class AboutYouModel : FormBase, IValidatableObject
{
    [Required(ErrorMessage = "Enter your full name")]
    [RegularExpression("^[A-Za-z\\s\\-']+$", ErrorMessage = "Full name can only contain letters, spaces, hyphens and apostrophes")]
    public string FullName { get; init; }

    [Phone(ErrorMessage = "Enter a valid telephone number")]
    [Required(ErrorMessage = "Enter your telephone number")]
    [RegularExpression(@"^[+0-9\s\-()]{6,20}$", ErrorMessage = "Telephone number must be between 6 and 20 characters and can only contain numbers, spaces, hyphens, brackets and an optional + for country code")]
    public string TelephoneNumber { get; init; } = string.Empty;

    [Required(ErrorMessage = "Enter your email address")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    public string EmailAddress { get; init; } = string.Empty;

    public AddressModel Address { get; set; } = new AddressModel();

    [ExcludeFromSummary]
    public int? Day { get; set; }

    [ExcludeFromSummary]
    public int? Month { get; set; }

    [ExcludeFromSummary]
    public int? Year { get; set; }

    [Required(ErrorMessage = "Enter your date of birth")]
    public DateOnly? DateOfBirth
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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateOfBirth is null)
        {
            yield return new ValidationResult(
                "Enter a valid date of birth",
                [nameof(DateOfBirth)]);
            yield break;
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (DateOfBirth.Value >= today)
        {
            yield return new ValidationResult(
                "Date of birth must be in the past",
                [nameof(DateOfBirth)]);
        }
    }
}
