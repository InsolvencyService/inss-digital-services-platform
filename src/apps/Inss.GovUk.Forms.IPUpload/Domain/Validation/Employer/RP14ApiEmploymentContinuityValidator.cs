using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiEmploymentContinuityValidator : AbstractValidator<Domain.Employer.Api.RP14EmployeesEmployeesClaimingContinuity>
{
    public RP14ApiEmploymentContinuityValidator()
    {
        RuleFor(p => p.EmployerName).ValidateContinuityEmployerName();
        RuleFor(p => p.Address).SetValidator(new RP14ApiAddressValidator("Employment continuity"));
    }
}