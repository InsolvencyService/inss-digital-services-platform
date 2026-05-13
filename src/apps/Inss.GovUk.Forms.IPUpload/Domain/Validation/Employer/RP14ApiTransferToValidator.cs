using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14ApiTransferToValidator : AbstractValidator<Domain.Employer.Api.RP14TransferDetailsTransferTo>
{
    public RP14ApiTransferToValidator()
    {
        RuleFor(p => p.Name).ValidateTransferToName();
        RuleFor(p => p.Address).SetValidator(new RP14ApiAddressValidator("Transfers"));
    }
}