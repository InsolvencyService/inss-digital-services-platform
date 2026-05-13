using FluentValidation;
using Inss.GovUk.Forms.IPUpload.Domain.Employee.Api;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

public sealed class RP14AApiArrearsOfPayValidator : AbstractValidator<RP14AEmployeePayDetailsArrearsOfPayPeriod>
{
    public RP14AApiArrearsOfPayValidator()
    {
        RuleFor(p => p.AOPOwed).ValidateMoney(RP14AValidationInfo.InvalidAopOwedFormat);
        RuleFor(p => p.Period.StartDate).ValidateStartEndDates(p => p.Period.EndDate, RP14AValidationInfo.InvalidAopDates);
    }
}