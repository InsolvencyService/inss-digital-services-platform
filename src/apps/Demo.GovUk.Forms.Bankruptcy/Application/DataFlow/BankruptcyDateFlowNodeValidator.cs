using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.Bankruptcy.Application.DataFlow;

public sealed class BankruptcyDateFlowNodeValidator : IFlowNodeValidator
{
    public async ValueTask<ValidationResult[]> ValidateAsync(FlowNodeContext context)
    {
        ValidationResult[] baseValidationResults = await DefaultFlowNodeValidator.Default.ValidateAsync(context);
        List<ValidationResult> validationResults = baseValidationResults.ToList();
        
        DateModel bankruptcyDate = context.CurrentPage.As<DateModel>();

        if (bankruptcyDate.Value == DateOnly.MinValue)
        {
            validationResults.AddResult("Enter a valid date", [nameof(bankruptcyDate.Value)]);
        }
        else
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (bankruptcyDate.Value >= today || bankruptcyDate.Value < today.AddYears(-1))
            {
                validationResults.AddResult("The bankruptcy date must be in the past 12 months", [nameof(bankruptcyDate.Value)]);
            }
        }

        return await ValueTask.FromResult(validationResults.ToArray());
    }
}