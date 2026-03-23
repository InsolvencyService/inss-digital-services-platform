using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.Business.Application.DataFlow;

public sealed class DebtorAmountFlowNodeValidator : IFlowNodeValidator
{
    private const int Minimum = 1000;
    private const int Maximum = 100_000;
    private const string ErrorMessage = "Only add debtors who owe between £1,000 and £100,000 inclusive";
    
    public async ValueTask<ValidationResult[]> ValidateAsync(ValidateContext context)
    {
        ValidationResult[] baseValidationResults = await DefaultFlowNodeValidator.Default.ValidateAsync(context);
        List<ValidationResult> validationResults = baseValidationResults.ToList();
        
        MoneyModel money = context.Page.As<MoneyModel>();
        
        if (money.Amount is < Minimum or > Maximum)
        {
            validationResults.AddResult(ErrorMessage, [nameof(money.Amount)]);
        }
        
        return await ValueTask.FromResult(validationResults.ToArray());
    }
}