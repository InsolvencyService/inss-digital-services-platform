namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public interface IBaseValidator
{
    Task<ValidatorContext> ValidateAsync();
}