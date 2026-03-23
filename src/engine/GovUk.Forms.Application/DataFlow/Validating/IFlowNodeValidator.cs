using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Application.DataFlow.Validating;

public interface IFlowNodeValidator
{
    ValueTask<ValidationResult[]> ValidateAsync(ValidateContext context);
}