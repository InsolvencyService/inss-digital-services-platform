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

public sealed class DefaultPageValidator : IPageValidator
{
    public static readonly IPageValidator Default = new DefaultPageValidator();
    private const bool AllProperties = true;
    
    private DefaultPageValidator()
    {
    }
    
    public ValueTask<ValidationResult[]> ValidateAsync(ValidatePageContext context)
    {
        List<ValidationResult> validationResults = [];
        ValidationContext validationContext = new(context.CurrentPage);
        Validator.TryValidateObject(context.CurrentPage, validationContext, validationResults, AllProperties);
        return ValueTask.FromResult(validationResults.ToArray());
    }
}