namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public interface IValidationFactory
{
    IBaseValidator Create(object model);
}