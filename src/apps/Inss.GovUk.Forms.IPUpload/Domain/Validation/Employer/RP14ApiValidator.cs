using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiValidator : AbstractValidator<Domain.Employer.Api.RP14>
{
    public RP14ApiValidator()
    {
        RuleFor(p => p.NameOfBusiness).ValidateBusinessName();
        RuleFor(p => p.CompanyNumber).ValidateCompanyNumber();
        RuleFor(p => p.Address).SetValidator(new RP14ApiAddressValidator("Business"));
        RuleFor(p => p.SICCode).ValidateSIC();
        RuleForEach(p => p.Directors.Director).SetValidator(new RP14ApiDirectorValidator());
        RuleForEach(p => p.Shareholders).SetValidator(new RP14ApiShareholderValidator());
        RuleForEach(p => p.AssociatedCompanies.AssociatedCompany).SetValidator(new RP14ApiAssociatedCompanyValidator());
        RuleFor(p => p.Employees.EmployeesClaimingContinuity).SetValidator(new RP14ApiEmploymentContinuityValidator());
        RuleFor(p => p.TransferDetails.TransferTo).SetValidator(new RP14ApiTransferToValidator());
        RuleFor(p => p.PayRecordsContact).SetValidator(new RP14ApiPayRecordsContactValidator());
    }
}