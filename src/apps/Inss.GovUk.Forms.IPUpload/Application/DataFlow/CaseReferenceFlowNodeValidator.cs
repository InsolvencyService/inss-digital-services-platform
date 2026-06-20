using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Application.Extensions;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public partial class CaseReferenceFlowNodeValidator : IFlowNodeValidator
{
    private readonly ICaseReferenceService _caseReferenceService;
    private const int CaseReferenceLength = 10;
    private const string CaseReferenceKey = "CaseReference.Value";
    
    public CaseReferenceFlowNodeValidator(ICaseReferenceService caseReferenceService)
    {
        _caseReferenceService = caseReferenceService;
    }

    public async ValueTask<ValidationResult[]> ValidateAsync(FlowNodeContext context)
    {
        ValidationResult[] baseValidationResults = await DefaultFlowNodeValidator.Default.ValidateAsync(context);
        List<ValidationResult> validationResults = baseValidationResults.ToList();
        CheckCaseReferenceModel checkCaseReference = context.CurrentPage.As<CheckCaseReferenceModel>();
        string caseReference = checkCaseReference.CaseReference.Value;

        if (string.IsNullOrWhiteSpace(caseReference))
        {
            validationResults.AddResult("Enter a reference number like CN12345678", [CaseReferenceKey]);
        }
        else
        {
            bool isValid = true;

            switch (caseReference.Length)
            {
                case < CaseReferenceLength:
                    isValid = false;
                    validationResults.AddResult("The case reference number is too short", [CaseReferenceKey]);
                    break;
                case > CaseReferenceLength:
                    isValid = false;
                    validationResults.AddResult("The case reference number is too long", [CaseReferenceKey]);
                    break;
            }

            if (isValid && !CaseReferenceNumberRegex().IsMatch(caseReference))
            {
                isValid = false;
                validationResults.AddResult("The case reference number is not in the correct format", [CaseReferenceKey]);
            }

            if (isValid)
            {
                CaseDetailModel? caseDetail = await _caseReferenceService.GetCaseDetailsAsync(caseReference);

                if (caseDetail is null)
                {
                    validationResults.AddResult(
                        "The case reference number you entered has not been linked to a valid employer", [CaseReferenceKey]);
                }
                else 
                {
                    EmployerDetailsModel employerDetails = context.Section.Pages.GetFirstOf<EmployerDetailsModel>();
                    employerDetails.CaseReference = caseDetail.CaseReference ?? string.Empty;
                    employerDetails.EmployerName = caseDetail.CompanyName ?? string.Empty;                    
                }
            }
        }

        return await ValueTask.FromResult(validationResults.ToArray());
    }

    [GeneratedRegex("^CN[0-9]{8}|cn[0-9]{8}|Cn[0-9]{8}|cN[0-9]{8}$", RegexOptions.Compiled)]
    private static partial Regex CaseReferenceNumberRegex();
}



