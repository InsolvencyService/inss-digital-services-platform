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
        RuleFor(p => p.MoneyOwedToEmployer).ValidateMoney(EmployeeValidationInfo.InvalidMoneyOwedToEmployerFormat);
        RuleFor(p => p.StartDate).ValidateStartEndDates(p => p.EndDate, EmployeeValidationInfo.InvalidEmployeeEmploymentDates);
        RuleFor(p => p.PayDetails.BasicPayPerWeek).ValidateMoney(EmployeePayValidationInfo.InvalidEmployeeBasicPayFormat);
        
        RuleFor(p => p.Holiday.HolidayContractedEntitlementDays).ValidateHoliday(EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementFormat);
        RuleFor(p => p.Holiday.HolidayContractedEntitlementDays).ValidateDaysInYear(EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementRange);
        RuleFor(p => p.Holiday.HolidayDaysCarriedForward).ValidateHoliday(EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardFormat);
        RuleFor(p => p.Holiday.HolidayDaysCarriedForward).ValidateDaysInYear(EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardRange);
        RuleFor(p => p.Holiday.HolidayDaysTaken).ValidateHoliday(EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenFormat);
        RuleFor(p => p.Holiday.HolidayDaysTaken).ValidateDaysInYear(EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenRange);
        RuleFor(p => p.Holiday.NoDaysHolidayOwed).ValidateHoliday(EmployeeHolidayValidationInfo.InvalidHolidayOwedFormat);
        RuleFor(p => p.Holiday.NoDaysHolidayOwed).ValidateDaysInYear(EmployeeHolidayValidationInfo.InvalidHolidayOwedRange);
        RuleFor(p => p.Holiday).SetValidator(new RP14AApiEmployeeHolidayValidator());
    }
}