using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiPayRecordsContactValidator : AbstractValidator<Domain.Employer.Api.RP14PayRecordsContact>
{
    public RP14ApiPayRecordsContactValidator()
    {
        RuleFor(p => p.Name).ValidatePayRecordContactName();
        RuleFor(p => p.PhoneNumber).ValidatePayRecordContactPhone();
        RuleFor(p => p.Address).SetValidator(new RP14ApiAddressValidator("Pay records contact"));
    }
}