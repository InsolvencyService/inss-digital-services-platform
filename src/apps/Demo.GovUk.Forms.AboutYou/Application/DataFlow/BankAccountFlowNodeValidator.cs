using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.AboutYou.Application.DataFlow;

public sealed class BankAccountFlowNodeValidator : IFlowNodeValidator
{
    public async ValueTask<ValidationResult[]> ValidateAsync(ValidateContext context)
    {
        ValidationResult[] baseValidationResults = await DefaultFlowNodeValidator.Default.ValidateAsync(context);
        List<ValidationResult> validationResults = baseValidationResults.ToList();
        BankAccountModel bankAccount = context.Page.As<BankAccountModel>();

        if (bankAccount is { AccountNumber: "12345678", SortCode: "11-22-33" })
        {
            validationResults.AddResult(
                "The bank account details are invalid", [nameof(bankAccount.AccountNumber), nameof(bankAccount.SortCode)]);
        }

        return await ValueTask.FromResult(validationResults.ToArray());
    }
}