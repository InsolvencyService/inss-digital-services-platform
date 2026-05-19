using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiShareholderValidator : AbstractValidator<Inss.Common.IPUpload.Employer.Api.RP14Shareholder>
{
    public RP14ApiShareholderValidator()
    {
        RuleFor(p => p.Name.FullName).ValidateShareholderName();
        RuleFor(p => p.Percentage).ValidateShareholderPercentage();
    }
}