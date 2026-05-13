using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiAddressValidator : AbstractValidator<Domain.Employer.Api.AddressType>
{
    public RP14ApiAddressValidator(string category)
    {
        RuleFor(p => p.Line)
            .Must(p => p.Length <= 4)
            .OverridePropertyName(RP14ValidationInfo.InvalidLinesLength.PropertyFormat.Replace("[CATEGORY]", category))
            .WithMessage(RP14ValidationInfo.InvalidPayRecordPhoneLength.ErrorFormat);
        RuleForEach(p => p.Line)
            .MaximumLength(35)
            .OverridePropertyName(RP14ValidationInfo.InvalidLineLength.PropertyFormat.Replace("[CATEGORY]", category))
            .WithMessage(RP14ValidationInfo.InvalidLineLength.ErrorFormat);
        RuleFor(p => p.Town)
            .MaximumLength(35)
            .OverridePropertyName(RP14ValidationInfo.InvalidTownLength.PropertyFormat.Replace("[CATEGORY]", category))
            .WithMessage(RP14ValidationInfo.InvalidTownLength.ErrorFormat);
        RuleFor(p => p.County)
            .MaximumLength(35)
            .OverridePropertyName(RP14ValidationInfo.InvalidCountyLength.PropertyFormat.Replace("[CATEGORY]", category))
            .WithMessage(RP14ValidationInfo.InvalidCountyLength.ErrorFormat);
        RuleFor(p => p.Postcode)
            .MaximumLength(10)
            .OverridePropertyName(RP14ValidationInfo.InvalidPostcodeLength.PropertyFormat.Replace("[CATEGORY]", category))
            .WithMessage(RP14ValidationInfo.InvalidPostcodeLength.ErrorFormat);
        RuleFor(p => p.Country)
            .MaximumLength(10)
            .OverridePropertyName(RP14ValidationInfo.InvalidCountryLength.PropertyFormat.Replace("[CATEGORY]", category))
            .WithMessage(RP14ValidationInfo.InvalidCountryLength.ErrorFormat);
    }
}