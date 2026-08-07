namespace Inss.Platform.Ipus.Domain.Validation;

public abstract class ValidatorContext
{
    public List<Error> Errors { get; } = [];

    public abstract void AddError(ValidationInfo validationInfo, string? value);

    protected void AddError(Error error)
    {
        Errors.Add(error);
    }
}