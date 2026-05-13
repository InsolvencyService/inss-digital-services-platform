using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiAddressValidator : AbstractValidator<Domain.Employer.Api.AddressType>
{
    public RP14ApiAddressValidator(string category)
    {
        RuleFor(p => p.Line).ValidateAddressLines(category);
        RuleForEach(p => p.Line).ValidateAddressLine(category);
        RuleFor(p => p.Town).ValidateAddressTown(category);
        RuleFor(p => p.County).ValidateAddressCounty(category);
        RuleFor(p => p.Postcode).ValidateAddressPostcode(category);
        RuleFor(p => p.Country).ValidateAddressCountry(category);
    }
}