using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.RP14A;

public sealed class RP14AApiValidator : AbstractValidator<Employee.Api.RP14A>
{
    public RP14AApiValidator()
    {
        RuleFor(p => p.Header.CaseReference).ValidateCaseReference();
        RuleFor(p => p.EmployerName).ValidateEmployerName();
    }
}