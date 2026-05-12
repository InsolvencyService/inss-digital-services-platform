using System.Globalization;
using FluentValidation;
using Inss.GovUk.Forms.IPUpload.Domain.Employee.Api;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class CaseReferenceValidator : AbstractValidator<string>
{
    public CaseReferenceValidator()
    {
        RuleFor(p => p)
            .NotEmpty()
            .OverridePropertyName(RP14AValidation.MissingCaseReference.PropertyFormat)
            .WithMessage(RP14AValidation.MissingCaseReference.ErrorFormat);
        RuleFor(p => p)
            .Matches(ValidatorConstants.CaseReferenceFormat)
            .OverridePropertyName(RP14AValidation.InvalidCaseReferenceFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidCaseReferenceFormat.ErrorFormat);
        RuleFor(p => p)
            .MaximumLength(12)
            .OverridePropertyName(RP14AValidation.InvalidCaseReferenceLength.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidCaseReferenceLength.ErrorFormat);
    }
}

public sealed class RP14AValidator2 : AbstractValidator<RP14A>
{
    public RP14AValidator2()
    {
        RuleFor(p => p.Header.CaseReference).ValidateCaseReference();
        /*RuleFor(p => p.Header.CaseReference)
            .NotEmpty()
            .OverridePropertyName(RP14AValidation.MissingCaseReference.PropertyFormat)
            .WithMessage(RP14AValidation.MissingCaseReference.ErrorFormat);
        RuleFor(p => p.Header.CaseReference)
            .Matches(ValidatorConstants.CaseReferenceFormat)
            .OverridePropertyName(RP14AValidation.InvalidCaseReferenceFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidCaseReferenceFormat.ErrorFormat);
        RuleFor(p => p.Header.CaseReference)
            .MaximumLength(12)
            .OverridePropertyName(RP14AValidation.InvalidCaseReferenceLength.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidCaseReferenceLength.ErrorFormat);*/
        
        RuleFor(p => p.EmployerName)
            .MaximumLength(99)
            .OverridePropertyName(RP14AValidation.InvalidEmployerNameLength.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidEmployerNameLength.ErrorFormat);
    }
}
public class RP14AApiEmployeeValidator : AbstractValidator<RP14AEmployee>
{
    public RP14AApiEmployeeValidator()
    {
        RuleFor(p => p.EmployeeName.Surname)
            .NotEmpty()
            .OverridePropertyName(RP14AValidation.MissingEmployeeSurname.PropertyFormat)
            .WithMessage(RP14AValidation.MissingEmployeeSurname.ErrorFormat);
        RuleFor(p => p.EmployeeName.Surname)
            .MaximumLength(99)
            .OverridePropertyName(RP14AValidation.InvalidEmployeeSurnameLength.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidEmployeeSurnameLength.ErrorFormat);
        
        RuleFor(p => p.NINO)
            .NotEmpty()
            .OverridePropertyName(RP14AValidation.MissingNino.PropertyFormat)
            .WithMessage(RP14AValidation.MissingNino.ErrorFormat);
        RuleFor(p => p.NINO)
            .Matches(ValidatorConstants.NinoFormat)
            .OverridePropertyName(RP14AValidation.InvalidNinoFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidNinoFormat.ErrorFormat);

        RuleForEach(p => p.PayDetails.ArrearsOfPay).SetValidator(new RP14AApiArrearsOfPayValidator());
        
        RuleFor(p => p.MoneyOwedToEmployer.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidMoneyOwedFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidMoneyOwedFormat.ErrorFormat);
        
        RuleFor(p => p).Custom((p, c) =>
        {
            if (p.StartDate > p.EndDate)
            {
                c.AddFailure(RP14AValidation.InvalidEmploymentDates.PropertyFormat, RP14AValidation.InvalidEmploymentDates.ErrorFormat);
            }
        });
        
        RuleFor(p => p.PayDetails.BasicPayPerWeek.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidBasicPayFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidBasicPayFormat.ErrorFormat);
        
        
        RuleFor(p => p.Holiday.HolidayContractedEntitlementDays.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidHolidayEntitlementFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayEntitlementFormat.ErrorFormat);
        RuleFor(p => p.Holiday.HolidayContractedEntitlementDays)
            .InclusiveBetween(0, 365)
            .OverridePropertyName(RP14AValidation.InvalidHolidayEntitlementRange.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayEntitlementRange.ErrorFormat);
        
        RuleFor(p => p.Holiday.HolidayDaysCarriedForward.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidHolidayCarriedForwardFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayCarriedForwardFormat.ErrorFormat);
        RuleFor(p => p.Holiday.HolidayDaysCarriedForward)
            .InclusiveBetween(0, 365)
            .OverridePropertyName(RP14AValidation.InvalidHolidayCarriedForwardRange.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayCarriedForwardRange.ErrorFormat);

        RuleFor(p => p.Holiday.HolidayDaysTaken.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidHolidayDaysTakenFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayDaysTakenFormat.ErrorFormat);
        RuleFor(p => p.Holiday.HolidayDaysTaken)
            .InclusiveBetween(0, 365)
            .OverridePropertyName(RP14AValidation.InvalidHolidayDaysTakenRange.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayDaysTakenRange.ErrorFormat);

        RuleFor(p => p.Holiday.NoDaysHolidayOwed.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidHolidayOwedFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayOwedFormat.ErrorFormat);
        RuleFor(p => p.Holiday.NoDaysHolidayOwed)
            .InclusiveBetween(0, 365)
            .OverridePropertyName(RP14AValidation.InvalidHolidayOwedRange.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayOwedRange.ErrorFormat);

        RuleFor(p => p.Holiday).SetValidator(new RP14AApiEmployeeHolidayValidator());
    }
}

public sealed class RP14AApiArrearsOfPayValidator : AbstractValidator<RP14AEmployeePayDetailsArrearsOfPayPeriod>
{
    public RP14AApiArrearsOfPayValidator()
    {
        RuleFor(p => p.AOPOwed.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidAopOwedFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidAopOwedFormat.ErrorFormat);
        RuleFor(p => p.Period).Custom((p, c) =>
        {
            if (p.StartDate > p.EndDate)
            {
                c.AddFailure(RP14AValidation.InvalidAopDates.PropertyFormat, RP14AValidation.InvalidAopDates.ErrorFormat);
            }
        });
    }
}

public sealed class RP14AApiEmployeeHolidayValidator : AbstractValidator<RP14AEmployeeHoliday>
{
    public RP14AApiEmployeeHolidayValidator()
    {
        RuleForEach(p => p.HolidayNotPaid).Custom((p, c) =>
        {
            if (p.StartDate > p.EndDate)
            {
                c.AddFailure(RP14AValidation.InvalidHolidayNotPaidRange.PropertyFormat, RP14AValidation.InvalidHolidayNotPaidRange.ErrorFormat);
            }
        });
    }
}