using FluentValidation;
using Inss.Common.IPUpload.Employee.Spreadsheet;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

public sealed class RP14ASpreadsheetHolidayNotPaidValidator : AbstractValidator<RP14AEmployeeHolidayHolidayNotPaid>
{
    public RP14ASpreadsheetHolidayNotPaidValidator()
    {
        RuleFor(p => p.Holiday1.Holiday1StartDate).ValidateStartEndDates(
            p => p.Holiday1.Holiday1EndDate, EmployeeHolidayValidationInfo.InvalidHolidayNotPaidRange);
        RuleFor(p => p.Holiday2.Holiday2StartDate).ValidateStartEndDates(
            p => p.Holiday2.Holiday2EndDate, EmployeeHolidayValidationInfo.InvalidHolidayNotPaidRange);
        RuleFor(p => p.Holiday3.Holiday3StartDate).ValidateStartEndDates(
            p => p.Holiday3.Holiday3EndDate, EmployeeHolidayValidationInfo.InvalidHolidayNotPaidRange);
    }
}