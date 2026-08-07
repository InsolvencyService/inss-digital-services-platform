namespace Inss.Platform.Ipus.Domain.Validation;

public interface IValidationFactory
{
    IBaseValidator Create(object model);
}