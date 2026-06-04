namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class EmployerValidatorContext : ValidatorContext
{
    public override void AddError(ValidationInfo validationInfo, string? value)
    {
        AddError(new EmployerError { Info = validationInfo });
    }
}