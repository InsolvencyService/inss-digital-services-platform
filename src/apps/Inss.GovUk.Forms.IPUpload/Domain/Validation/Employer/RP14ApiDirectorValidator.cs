using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiDirectorValidator : AbstractValidator<Domain.Employer.Api.RP14DirectorsDirector>
{
    public RP14ApiDirectorValidator()
    {
        RuleFor(p => p.NINO).ValidateDirectorNino();
    }
}