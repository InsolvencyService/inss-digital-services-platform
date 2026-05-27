using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiDirectorValidator : AbstractValidator<Inss.Common.IPUpload.Employer.Api.RP14DirectorsDirector>
{
    public RP14ApiDirectorValidator()
    {
        RuleFor(p => p.Name.Initials).ValidateDirectorInitials().When(p => p.Name is not null);
        RuleFor(p => p.Name.Surname).ValidateDirectorSurname().When(p => p.Name is not null);
        RuleFor(p => p.NINO).ValidateDirectorNino();
    }
}