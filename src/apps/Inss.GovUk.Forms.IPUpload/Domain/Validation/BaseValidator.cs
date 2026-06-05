using System.Text.RegularExpressions;
using Inss.GovUk.Forms.IPUpload.Application.Services;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public abstract partial class BaseValidator : IBaseValidator
{
    private readonly ICaseReferenceService _caseReferenceService;

    protected BaseValidator(ICaseReferenceService caseReferenceService)
    {
        _caseReferenceService = caseReferenceService;
    }

    public abstract Task<ValidatorContext> ValidateAsync();
    
    protected async Task ValidateCaseReferenceAsync(ValidatorContext context, string caseReference)
    {
        if (string.IsNullOrWhiteSpace(caseReference))
        {
            context.AddError(CaseValidationInfo.MissingCaseReference(), caseReference);
        }
        else
        {
            bool isValid = true;
            
            if (caseReference.Length > 10)
            {
                isValid = false;
                context.AddError(CaseValidationInfo.InvalidCaseReferenceLength(), caseReference);
            }

            if (isValid && !CaseRefFormatRegex().IsMatch(caseReference))
            {
                isValid = false;
                context.AddError(CaseValidationInfo.InvalidCaseReferenceFormat(), caseReference);
            }

            if (isValid)
            {
                bool caseReferenceExists = await _caseReferenceService.CheckExistsAsync(caseReference);

                if (!caseReferenceExists)
                {
                    context.AddError(CaseValidationInfo.UnknownCaseReference(), caseReference);
                }
            }
        }
    }
    
    [GeneratedRegex(ValidationInfo.CaseReferenceFormat)]
    private static partial Regex CaseRefFormatRegex();
}