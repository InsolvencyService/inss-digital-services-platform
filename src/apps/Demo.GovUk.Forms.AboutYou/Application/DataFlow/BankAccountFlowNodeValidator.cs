using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.AboutYou.Application.DataFlow;

public sealed partial class BankAccountFlowNodeValidator : IFlowNodeValidator
{
    private static readonly Regex _regex = BuildingSocietyRollNumberRegex();
    
    public async ValueTask<ValidationResult[]> ValidateAsync(FlowNodeContext context)
    {
        ValidationResult[] baseValidationResults = await DefaultFlowNodeValidator.Default.ValidateAsync(context);
        List<ValidationResult> validationResults = baseValidationResults.ToList();
        BankAccountModel bankAccount = context.CurrentPage.As<BankAccountModel>();

        if (bankAccount is { AccountNumber: "12345678", SortCode: "11-22-33" })
        {
            validationResults.AddResult(
                "The bank account details are invalid", [nameof(bankAccount.AccountNumber), nameof(bankAccount.SortCode)]);
        }

        if (!string.IsNullOrWhiteSpace(bankAccount.BuildingSocietyRollNumber))
        {
            if (bankAccount.BuildingSocietyRollNumber.Length is < 1 or > 18)
            {
                validationResults.AddResult(
                    "Building society roll number must be between 1 and 18 characters", [nameof(bankAccount.BuildingSocietyRollNumber)]);
            }
            
            if (!_regex.IsMatch(bankAccount.BuildingSocietyRollNumber))
            {
                validationResults.AddResult(
                    "Building society roll number must only include letters a to z, numbers, hyphens, spaces, forward slashes and full stops", 
                    [nameof(bankAccount.BuildingSocietyRollNumber)]);
            }
        }
        
        return await ValueTask.FromResult(validationResults.ToArray());
    }
    
    [GeneratedRegex(@"^[A-Za-z0-9\-\/\. ]+$", RegexOptions.Compiled)]
    private static partial Regex BuildingSocietyRollNumberRegex();
}