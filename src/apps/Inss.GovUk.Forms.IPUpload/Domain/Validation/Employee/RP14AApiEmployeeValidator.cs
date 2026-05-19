using FluentValidation;
using Inss.Common.IPUpload.Employee.Api;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

public class RP14AApiEmployeeValidator : AbstractValidator<RP14AEmployee>
{
    public RP14AApiEmployeeValidator()
    {
        RuleFor(p => p.EmployeeName.Surname).ValidateEmployeeSurname();
        RuleFor(p => p.NINO).ValidateNino();
        RuleForEach(p => p.PayDetails.ArrearsOfPay).SetValidator(new RP14AApiArrearsOfPayValidator());
        RuleFor(p => p.MoneyOwedToEmployer).ValidateMoney(RP14AValidationInfo.InvalidMoneyOwedFormat);
        RuleFor(p => p.StartDate).ValidateStartEndDates(p => p.EndDate, RP14AValidationInfo.InvalidEmploymentDates);
        RuleFor(p => p.PayDetails.BasicPayPerWeek).ValidateMoney(RP14AValidationInfo.InvalidBasicPayFormat);
        
        RuleFor(p => p.Holiday.HolidayContractedEntitlementDays).ValidateHoliday(RP14AValidationInfo.InvalidHolidayEntitlementFormat);
        RuleFor(p => p.Holiday.HolidayContractedEntitlementDays).ValidateDaysInYear(RP14AValidationInfo.InvalidHolidayEntitlementRange);
        RuleFor(p => p.Holiday.HolidayDaysCarriedForward).ValidateHoliday(RP14AValidationInfo.InvalidHolidayCarriedForwardFormat);
        RuleFor(p => p.Holiday.HolidayDaysCarriedForward).ValidateDaysInYear(RP14AValidationInfo.InvalidHolidayCarriedForwardRange);
        RuleFor(p => p.Holiday.HolidayDaysTaken).ValidateHoliday(RP14AValidationInfo.InvalidHolidayDaysTakenFormat);
        RuleFor(p => p.Holiday.HolidayDaysTaken).ValidateDaysInYear(RP14AValidationInfo.InvalidHolidayDaysTakenRange);
        RuleFor(p => p.Holiday.NoDaysHolidayOwed).ValidateHoliday(RP14AValidationInfo.InvalidHolidayOwedFormat);
        RuleFor(p => p.Holiday.NoDaysHolidayOwed).ValidateDaysInYear(RP14AValidationInfo.InvalidHolidayOwedRange);
        RuleFor(p => p.Holiday).SetValidator(new RP14AApiEmployeeHolidayValidator());
    }
}