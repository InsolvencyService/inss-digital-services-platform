using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Application.DataFlow.Validating;

public sealed class DefaultFlowNodeValidator : IFlowNodeValidator
{
    public static readonly IFlowNodeValidator Default = new DefaultFlowNodeValidator();
    private const bool AllProperties = true;
    
    private DefaultFlowNodeValidator()
    {
    }
    
    public ValueTask<ValidationResult[]> ValidateAsync(ValidateContext context)
    {
        List<ValidationResult> validationResults = [];
        ValidationContext validationContext = new(context.Page);
        Validator.TryValidateObject(context.Page, validationContext, validationResults, AllProperties);
        return ValueTask.FromResult(validationResults.ToArray());
    }
}