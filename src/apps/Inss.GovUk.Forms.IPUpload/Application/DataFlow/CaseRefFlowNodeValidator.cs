using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public partial class CaseRefFlowNodeValidator : IFlowNodeValidator
{
    private readonly ICaseReferenceService _caseReferenceService;

    public CaseRefFlowNodeValidator(ICaseReferenceService caseReferenceService)
    {
        _caseReferenceService = caseReferenceService;
    }

    public async ValueTask<ValidationResult[]> ValidateAsync(FlowNodeContext context)
    {
        ValidationResult[] baseValidationResults = await DefaultFlowNodeValidator.Default.ValidateAsync(context);
        List<ValidationResult> validationResults = baseValidationResults.ToList();
        RequiredTextModel caseReferenceNumber = context.CurrentPage.As<RequiredTextModel>();

        if (string.IsNullOrWhiteSpace(caseReferenceNumber.Value))
        {
            validationResults.AddResult("Enter a reference number like CN12345678", [nameof(caseReferenceNumber.Value)]);
        }
        else
        {
            bool isValid = true;

            if (caseReferenceNumber.Value.Length < 10)
            {
                isValid = false;
                validationResults.AddResult("The case reference number is too short", [nameof(caseReferenceNumber.Value)]);
            }
            if (caseReferenceNumber.Value.Length > 10)
            {
                isValid = false;
                validationResults.AddResult("The case reference number is too long", [nameof(caseReferenceNumber.Value)]);
            }

            if (isValid && !CaseReferenceNumberRegex().IsMatch(caseReferenceNumber.Value))
            {
                isValid = false;
                validationResults.AddResult("The case reference number is not in the correct format", [nameof(caseReferenceNumber.Value)]);
            }

            if (isValid)
            {
                CaseDetailModel? caseDetail = await _caseReferenceService.GetCaseDetailsAsync(caseReferenceNumber.Value.ToString());

                if (caseDetail is null)
                {
                    validationResults.AddResult(
                        "The case reference number you entered has not been linked to a valid employer",
                        [nameof(caseReferenceNumber.Value)]);
                }
                else 
                {
                    EmployerDetailsModel employerDetails = context.Section.Pages.GetFirstOf<EmployerDetailsModel>();
                    employerDetails.CaseRefNum = caseDetail.CaseReference ?? string.Empty;
                    employerDetails.EmployerName = caseDetail.CompanyName ?? string.Empty;                    
                }

            }
        }

        return await ValueTask.FromResult(validationResults.ToArray());
    }

    [GeneratedRegex(@"^CN[0-9]{8}|cn[0-9]{8}|Cn[0-9]{8}|cN[0-9]{8}$", RegexOptions.Compiled)]
    private static partial Regex CaseReferenceNumberRegex();

}



