namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public abstract class BaseValidator : IBaseValidator
{
    public abstract ValidatorContext Validate(EmployerDetailsModel employerDetails);
    
    protected static void ValidateCaseReference(ValidatorContext context, string caseReference, string actualCaseReference)
    {
        if (actualCaseReference != caseReference)
        {
            context.AddError(CaseValidationInfo.CaseReferenceMismatch(), caseReference);
        }
    }
}