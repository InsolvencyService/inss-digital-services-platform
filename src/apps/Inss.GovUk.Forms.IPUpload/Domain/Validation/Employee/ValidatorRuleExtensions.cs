using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;

// ReSharper disable UnusedMethodReturnValue.Global

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

internal static partial class ValidatorRuleExtensions
{
    extension<T>(IRuleBuilder<T, string> rule)
    {
        internal IRuleBuilderOptions<T, string> ValidateCaseReference()
        {
            return rule
                .NotEmpty()
                .OverridePropertyName(RP14AValidationInfo.MissingCaseReference.PropertyFormat)
                .WithMessage(RP14AValidationInfo.MissingCaseReference.ErrorFormat)
                .Matches(ValidationInfo.CaseReferenceFormat)
                .OverridePropertyName(RP14AValidationInfo.InvalidCaseReferenceFormat.PropertyFormat)
                .WithMessage(RP14AValidationInfo.InvalidCaseReferenceFormat.ErrorFormat)
                .MaximumLength(12)
                .OverridePropertyName(RP14AValidationInfo.InvalidCaseReferenceLength.PropertyFormat)
                .WithMessage(RP14AValidationInfo.InvalidCaseReferenceLength.ErrorFormat);
        }

        internal IRuleBuilderOptions<T, string> ValidateNino()
        {
            return rule
                .NotEmpty()
                .OverridePropertyName(RP14AValidationInfo.MissingNino.PropertyFormat)
                .WithMessage(RP14AValidationInfo.MissingNino.ErrorFormat).Matches(ValidationInfo.NinoFormat)
                .Matches(ValidationInfo.NinoFormat)
                .OverridePropertyName(RP14AValidationInfo.InvalidNinoFormat.PropertyFormat)
                .WithMessage(RP14AValidationInfo.InvalidNinoFormat.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateEmployerName()
        {
            return rule
                .MaximumLength(99)
                .OverridePropertyName(RP14AValidationInfo.InvalidEmployerNameLength.PropertyFormat)
                .WithMessage(RP14AValidationInfo.InvalidEmployerNameLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateEmployeeSurname()
        {
            return rule
                .NotEmpty()
                .OverridePropertyName(RP14AValidationInfo.MissingEmployeeSurname.PropertyFormat)
                .WithMessage(RP14AValidationInfo.MissingEmployeeSurname.ErrorFormat)
                .MaximumLength(99)
                .OverridePropertyName(RP14AValidationInfo.InvalidEmployeeSurnameLength.PropertyFormat)
                .WithMessage(RP14AValidationInfo.InvalidEmployeeSurnameLength.ErrorFormat);
        }
    }

    extension<T>(IRuleBuilder<T, decimal> rule)
    {
        internal IRuleBuilderOptions<T, decimal> ValidateMoney(ValidationInfo validationInfo)
        {
            return rule
                .Must(value => MoneyFormatRegex().IsMatch(value.ToString(CultureInfo.CurrentCulture)))
                .OverridePropertyName(validationInfo.PropertyFormat)
                .WithMessage(validationInfo.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, decimal> ValidateHoliday(ValidationInfo validationInfo)
        {
            return rule
                .Must(value => HolidayFormatRegex().IsMatch(value.ToString(CultureInfo.CurrentCulture)))
                .OverridePropertyName(validationInfo.PropertyFormat)
                .WithMessage(validationInfo.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, decimal> ValidateDaysInYear(ValidationInfo validationInfo)
        {
            return rule
                .InclusiveBetween(0, 365)
                .OverridePropertyName(validationInfo.PropertyFormat)
                .WithMessage(validationInfo.ErrorFormat);
        }
    }

    extension<T>(IRuleBuilder<T, DateTime> rule)
    {
        internal IRuleBuilderOptions<T, DateTime> ValidateStartEndDates(Func<T, DateTime> endDateProperty, ValidationInfo validationInfo)
        {
            return rule
                .Must((obj, currentValue) =>
                {
                    // If not set, ignore. The spreadsheet will still process but have empty/default values
                    if (currentValue == DateTime.MinValue)
                    {
                        return true;
                    }
                    
                    DateTime endDate = endDateProperty(obj);
                    return currentValue < endDate;
                })
                .OverridePropertyName(validationInfo.PropertyFormat)
                .WithMessage(validationInfo.ErrorFormat);
        }
    }
    
    [GeneratedRegex(ValidationInfo.MoneyFormat)]
    private static partial Regex MoneyFormatRegex();
    
    [GeneratedRegex(ValidationInfo.HolidayFormat)]
    private static partial Regex HolidayFormatRegex();
}