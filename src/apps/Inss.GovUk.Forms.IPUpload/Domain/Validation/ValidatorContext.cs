namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal abstract class ValidatorContext
{
    internal List<Error> Errors { get; } = [];

    internal abstract void AddError(ValidationInfo validationInfo, string? value);

    protected void AddError(Error error)
    {
        Errors.Add(error);
    }
}