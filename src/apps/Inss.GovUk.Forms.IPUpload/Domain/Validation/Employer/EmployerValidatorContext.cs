namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

internal sealed class EmployerValidatorContext : ValidatorContext
{
    internal override void AddError(ValidationInfo validationInfo, string? value)
    {
        AddError(new EmployerError { Info = validationInfo, Value = value });
    }
}