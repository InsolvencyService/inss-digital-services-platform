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
                .OverridePropertyName(CaseValidationInfo.MissingCaseReference.Property)
                .WithMessage(CaseValidationInfo.MissingCaseReference.Key)
                .Matches(ValidationInfo.CaseReferenceFormat)
                .OverridePropertyName(CaseValidationInfo.InvalidCaseReferenceFormat.Property)
                .WithMessage(CaseValidationInfo.InvalidCaseReferenceFormat.Key)
                .MaximumLength(12)
                .OverridePropertyName(CaseValidationInfo.InvalidCaseReferenceLength.Property)
                .WithMessage(CaseValidationInfo.InvalidCaseReferenceLength.Key);
        }

        internal IRuleBuilderOptions<T, string> ValidateNino()
        {
            return rule
                .NotEmpty()
                .OverridePropertyName(EmployeeValidationInfo.MissingEmployeeNino.Property)
                .WithMessage(EmployeeValidationInfo.MissingEmployeeNino.Key)
                .Matches(ValidationInfo.NinoFormat)
                .OverridePropertyName(EmployeeValidationInfo.InvalidEmployeeNinoFormat.Property)
                .WithMessage(EmployeeValidationInfo.InvalidEmployeeNinoFormat.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateEmployerName()
        {
            return rule
                .MaximumLength(99)
                .OverridePropertyName(EmployerValidationInfo.InvalidEmployerNameLength.Property)
                .WithMessage(EmployerValidationInfo.InvalidEmployerNameLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateEmployeeSurname()
        {
            return rule
                .NotEmpty()
                .OverridePropertyName(EmployeeValidationInfo.MissingEmployeeSurname.Property)
                .WithMessage(EmployeeValidationInfo.MissingEmployeeSurname.Key)
                .MaximumLength(99)
                .OverridePropertyName(EmployeeValidationInfo.InvalidEmployeeSurnameLength.Property)
                .WithMessage(EmployeeValidationInfo.InvalidEmployeeSurnameLength.Key);
        }
    }

    extension<T>(IRuleBuilder<T, decimal> rule)
    {
        internal IRuleBuilderOptions<T, decimal> ValidateMoney(ValidationInfo validationInfo)
        {
            return rule
                .Must(value => MoneyFormatRegex().IsMatch(value.ToString(CultureInfo.CurrentCulture)))
                .OverridePropertyName(validationInfo.Property)
                .WithMessage(validationInfo.Key);
        }
        
        internal IRuleBuilderOptions<T, decimal> ValidateHoliday(ValidationInfo validationInfo)
        {
            return rule
                .Must(value => HolidayFormatRegex().IsMatch(value.ToString(CultureInfo.CurrentCulture)))
                .OverridePropertyName(validationInfo.Property)
                .WithMessage(validationInfo.Key);
        }
        
        internal IRuleBuilderOptions<T, decimal> ValidateDaysInYear(ValidationInfo validationInfo)
        {
            return rule
                .InclusiveBetween(0, 365)
                .OverridePropertyName(validationInfo.Property)
                .WithMessage(validationInfo.Key);
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
                .OverridePropertyName(validationInfo.Property)
                .WithMessage(validationInfo.Key);
        }
    }
    
    [GeneratedRegex(ValidationInfo.MoneyFormat)]
    private static partial Regex MoneyFormatRegex();
    
    [GeneratedRegex(ValidationInfo.HolidayFormat)]
    private static partial Regex HolidayFormatRegex();
}