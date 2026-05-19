using FluentValidation;
using Inss.Common.IPUpload.Employee.Api;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

public sealed class RP14AApiEmployeeHolidayValidator : AbstractValidator<RP14AEmployeeHoliday>
{
    public RP14AApiEmployeeHolidayValidator()
    {
        RuleForEach(p => p.HolidayNotPaid).Custom((p, c) =>
        {
            if (p.StartDate > p.EndDate)
            {
                c.AddFailure(
                    RP14AValidationInfo.InvalidHolidayNotPaidRange.PropertyFormat, 
                    RP14AValidationInfo.InvalidHolidayNotPaidRange.ErrorFormat);
            }
        });
    }
}