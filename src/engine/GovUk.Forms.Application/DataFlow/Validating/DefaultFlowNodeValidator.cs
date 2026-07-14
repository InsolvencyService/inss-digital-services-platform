using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Application.DataFlow.Validating;

public sealed class DefaultFlowNodeValidator : IFlowNodeValidator
{
    public static readonly IFlowNodeValidator Default = new DefaultFlowNodeValidator();
    private const bool AllProperties = true;
    
    private DefaultFlowNodeValidator()
    {
    }
    
    public ValueTask<ValidationResult[]> ValidateAsync(FlowNodeContext context)
    {
        List<ValidationResult> validationResults = [];
        ValidationContext validationContext = new(context.CurrentPage);
        Validator.TryValidateObject(context.CurrentPage, validationContext, validationResults, AllProperties);
        return ValueTask.FromResult(validationResults.ToArray());
    }
}