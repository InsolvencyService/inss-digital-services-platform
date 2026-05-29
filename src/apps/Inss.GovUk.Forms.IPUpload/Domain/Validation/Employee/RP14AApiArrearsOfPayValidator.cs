using FluentValidation;
using Inss.Common.IPUpload.Employee.Api;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

public sealed class RP14AApiArrearsOfPayValidator : AbstractValidator<RP14AEmployeePayDetailsArrearsOfPayPeriod>
{
    public RP14AApiArrearsOfPayValidator()
    {
        RuleFor(p => p.AOPOwed).ValidateMoney(EmployeeValidationInfo.InvalidAopOwedFormat);
        RuleFor(p => p.Period.StartDate).ValidateStartEndDates(p => p.Period.EndDate, EmployeeValidationInfo.InvalidEmployeeAopDates);
    }
}