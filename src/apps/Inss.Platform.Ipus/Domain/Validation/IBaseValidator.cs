namespace Inss.Platform.Ipus.Domain.Validation;

public interface IBaseValidator
{
    ValidatorContext Validate(string caseReference);
}