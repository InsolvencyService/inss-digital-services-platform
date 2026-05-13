using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiShareholderValidator : AbstractValidator<Domain.Employer.Api.RP14Shareholder>
{
    public RP14ApiShareholderValidator()
    {
        RuleFor(p => p.Percentage).ValidateShareholderPercentage();
    }
}