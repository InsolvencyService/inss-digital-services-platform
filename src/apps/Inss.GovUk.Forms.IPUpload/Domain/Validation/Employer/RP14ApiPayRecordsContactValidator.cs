using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiPayRecordsContactValidator : AbstractValidator<Inss.Common.IPUpload.Employer.Api.RP14PayRecordsContact>
{
    public RP14ApiPayRecordsContactValidator()
    {
        RuleFor(p => p.Name).ValidatePayRecordContactName();
        RuleFor(p => p.PhoneNumber).ValidatePayRecordContactPhone();
        RuleFor(p => p.EmailAddress).ValidatePayRecordContactEmail();
        RuleFor(p => p.Address).SetValidator(new RP14ApiAddressValidator("Pay records contact"));
    }
}