using System.ComponentModel.DataAnnotations;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Extensions;
using Inss.Platform.Domain.Validation;

namespace Inss.Platform.Application.Validators;

public sealed class MaxLengthComponentValidator : IComponentValidator
{
    private readonly ValidationRule _validationRule;
    public const string ErrorMessageKey = "ErrorMessage";
    public const string PropertyKey = "Property";
    public const string MaxLengthKey = "MaxLength";
    
    public MaxLengthComponentValidator(ValidationRule validationRule)
    {
        _validationRule = validationRule;
    }
    
    public ValueTask<ValidationResult[]> ValidateAsync(ComponentContext context)
    {
        IValueComponent valueComponent = context.Component.As<IValueComponent>();
        int maxLength = _validationRule.Items.GetValue<int>(MaxLengthKey);
        
        if (valueComponent.Value?.Length > maxLength)
        {
            string errorMessage = _validationRule.Items.GetValue<string>(ErrorMessageKey);
            string property = _validationRule.Items.GetValue<string>(PropertyKey);
            return ValueTask.FromResult<ValidationResult[]>([new ValidationResult(errorMessage, [property])]);
        }

        return ValueTask.FromResult<ValidationResult[]>([]);
    }
}