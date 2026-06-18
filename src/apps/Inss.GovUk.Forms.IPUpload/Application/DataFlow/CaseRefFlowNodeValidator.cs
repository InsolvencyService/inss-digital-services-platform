using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
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
        RequiredTextModel caserefnum = context.CurrentPage.As<RequiredTextModel>();

        if (string.IsNullOrWhiteSpace(caserefnum.Value))
        {
            validationResults.AddResult("Enter a reference number like CN12345678", [nameof(caserefnum.Value)]);
        }
        else
        {
            bool isValid = true;

            if (caserefnum.Value.Length < 10)
            {
                isValid = false;
                validationResults.AddResult("The case reference number is too short", [nameof(caserefnum.Value)]);
            }
            if (caserefnum.Value.Length > 10)
            {
                isValid = false;
                validationResults.AddResult("The case reference number is too long", [nameof(caserefnum.Value)]);
            }

            if (isValid && !CaseReferenceNumberRegex().IsMatch(caserefnum.Value))
            {
                isValid = false;
                validationResults.AddResult("The case reference number is not in the correct format", [nameof(caserefnum.Value)]);
            }

            if (isValid)
            {
                CaseDetailModel? casedetailModel = await _caseReferenceService.GetEmployerDetailsAsync(caserefnum.Value.ToString());

                if (casedetailModel is null)
                {
                    validationResults.AddResult(
                        "The case reference number you entered has not been linked to a valid employer",
                        [nameof(caserefnum.Value)]);
                }
                else 
                {
                    EmployerDetailsModel employerdetails = context.Section.Pages.GetFirstOf<EmployerDetailsModel>();
                    employerdetails.CaseRefNum = casedetailModel.CaseReference ?? string.Empty;
                    employerdetails.EmployerName = casedetailModel.CompanyName ?? string.Empty;
                    
                }

            }
        }

        return await ValueTask.FromResult(validationResults.ToArray());
    }

    [GeneratedRegex(@"^CN[0-9]{8}|cn[0-9]{8}|Cn[0-9]{8}|cN[0-9]{8}$", RegexOptions.Compiled)]
    private static partial Regex CaseReferenceNumberRegex();

}



