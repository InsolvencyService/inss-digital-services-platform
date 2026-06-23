namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public interface IBaseValidator
{
    ValidatorContext Validate(EmployerDetailsModel employerDetails);
}