namespace Inss.Platform.Ipus.Domain.Validation;

public abstract class BaseValidator : IBaseValidator
{
    public abstract ValidatorContext Validate(string caseReference);
    
    protected static void ValidateCaseReference(ValidatorContext context, string caseReference, string actualCaseReference)
    {
        if (actualCaseReference != caseReference)
        {
            context.AddError(CaseValidationInfo.CaseReferenceMismatch(actualCaseReference), caseReference);
        }
    }
}