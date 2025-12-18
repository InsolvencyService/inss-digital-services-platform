using INSS.Platform.Portal.Domain.Attributes;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public sealed class DateOfBirthModel : BaseModel, IValidatableObject
{
    public DateOfBirthModel()
    {
        PathName = "date-of-birth";
        Title = "Date of Birth";
    }

    [ExcludeFromSummary]
    public int? Day { get; set; }

    [ExcludeFromSummary]
    public int? Month { get; set; }

    [ExcludeFromSummary]
    public int? Year { get; set; }

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
