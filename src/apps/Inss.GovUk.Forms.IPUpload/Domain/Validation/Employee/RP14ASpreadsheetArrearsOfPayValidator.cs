using FluentValidation;
using Inss.Common.IPUpload.Employee.Spreadsheet;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

public sealed class RP14ASpreadsheetArrearsOfPayValidator : AbstractValidator<RP14AEmployeePayDetailsArrearsOfPay>
{
    public RP14ASpreadsheetArrearsOfPayValidator()
    {
        RuleFor(p => p.ArrearsOfPayPeriod1.AOP1StartDate).ValidateStartEndDates(
            p => p.ArrearsOfPayPeriod1.AOP1EndDate, EmployeeValidationInfo.InvalidEmployeeAopDates);
        RuleFor(p => p.ArrearsOfPayPeriod2.AOP2StartDate).ValidateStartEndDates(
            p => p.ArrearsOfPayPeriod2.AOP2EndDate, EmployeeValidationInfo.InvalidEmployeeAopDates);
        RuleFor(p => p.ArrearsOfPayPeriod3.AOP3StartDate).ValidateStartEndDates(
            p => p.ArrearsOfPayPeriod3.AOP3EndDate, EmployeeValidationInfo.InvalidEmployeeAopDates);
        RuleFor(p => p.ArrearsOfPayPeriod4.AOP4StartDate).ValidateStartEndDates(
            p => p.ArrearsOfPayPeriod4.AOP4EndDate, EmployeeValidationInfo.InvalidEmployeeAopDates);
    }
}