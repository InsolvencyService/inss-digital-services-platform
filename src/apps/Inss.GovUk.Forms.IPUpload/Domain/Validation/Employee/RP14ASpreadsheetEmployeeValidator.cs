using FluentValidation;
using Inss.Common.IPUpload.Employee.Spreadsheet;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

public class RP14ASpreadsheetEmployeeValidator : AbstractValidator<RP14AEmployee>
{
    public RP14ASpreadsheetEmployeeValidator()
    {
        RuleFor(p => p.Header.CaseReference).ValidateCaseReference();
        RuleFor(p => p.EmployerName).ValidateEmployerName();
        RuleFor(p => p.EmployeeName.Surname).ValidateEmployeeSurname();
        RuleFor(p => p.NINO).ValidateNino();
        RuleFor(p => p.MoneyOwedToEmployer).ValidateMoney(EmployeeValidationInfo.InvalidMoneyOwedToEmployerFormat);
        RuleFor(p => p.StartDate).ValidateStartEndDates(p => p.EndDate, EmployeeValidationInfo.InvalidEmployeeEmploymentDates);

        RuleFor(p => p.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod1.AOPOwed1).ValidateMoney(EmployeeValidationInfo.InvalidAopOwedFormat);
        RuleFor(p => p.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod2.AOPOwed2).ValidateMoney(EmployeeValidationInfo.InvalidAopOwedFormat);
        RuleFor(p => p.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod3.AOPOwed3).ValidateMoney(EmployeeValidationInfo.InvalidAopOwedFormat);
        RuleFor(p => p.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod4.AOPOwed4).ValidateMoney(EmployeeValidationInfo.InvalidAopOwedFormat);
        RuleFor(p => p.PayDetails.ArrearsOfPay).SetValidator(new RP14ASpreadsheetArrearsOfPayValidator());
        RuleFor(p => p.PayDetails.BasicPayPerWeek).ValidateMoney(EmployeePayValidationInfo.InvalidEmployeeBasicPayFormat);
        
        RuleFor(p => p.Holiday.HolidayContractedEntitlementDays).ValidateHoliday(EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementFormat);
        RuleFor(p => p.Holiday.HolidayContractedEntitlementDays).ValidateDaysInYear(EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementRange);
        RuleFor(p => p.Holiday.HolidayDaysCarriedForward).ValidateHoliday(EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardFormat);
        RuleFor(p => p.Holiday.HolidayDaysCarriedForward).ValidateDaysInYear(EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardRange);
        RuleFor(p => p.Holiday.HolidayDaysTaken).ValidateHoliday(EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenFormat);
        RuleFor(p => p.Holiday.HolidayDaysTaken).ValidateDaysInYear(EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenRange);
        RuleFor(p => p.Holiday.NoDaysHolidayOwed).ValidateHoliday(EmployeeHolidayValidationInfo.InvalidHolidayOwedFormat);
        RuleFor(p => p.Holiday.NoDaysHolidayOwed).ValidateDaysInYear(EmployeeHolidayValidationInfo.InvalidHolidayOwedRange);
        RuleFor(p => p.Holiday.HolidayNotPaid).SetValidator(new RP14ASpreadsheetHolidayNotPaidValidator());
    }
}