using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiInsolvencyPractitionerValidator : AbstractValidator<Domain.Employer.Api.RP14InsolvencyPractitioner>
{
    public RP14ApiInsolvencyPractitionerValidator()
    {
        RuleFor(p => p.RegistrationNumber).ValidateIPRegistrationNumber();
        RuleFor(p => p.FirmName).ValidateIPFirmName();
        RuleFor(p => p.Address).SetValidator(new RP14ApiAddressValidator("Insolvency practitioner"));
    }
}