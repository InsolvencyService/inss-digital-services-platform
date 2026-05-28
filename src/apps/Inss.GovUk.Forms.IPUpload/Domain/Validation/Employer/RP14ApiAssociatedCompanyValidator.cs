using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiAssociatedCompanyValidator : AbstractValidator<Inss.Common.IPUpload.Employer.Api.RP14AssociatedCompaniesAssociatedCompany>
{
    public RP14ApiAssociatedCompanyValidator()
    {
        RuleFor(p => p.CompanyName).ValidateAssociatedCompanyName();
        RuleFor(p => p.CompanyNumber).ValidateAssociatedCompanyNumber();
        RuleFor(p => p.ReasonForAssociation).ValidateAssociationReason();
        RuleFor(p => p.Address).SetValidator(new RP14ApiAddressValidator("Associated company"));
    }
}