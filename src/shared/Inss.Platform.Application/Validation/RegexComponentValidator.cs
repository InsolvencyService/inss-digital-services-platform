using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Extensions;
using Inss.Platform.Domain.Validation;

namespace Inss.Platform.Application.Validation;

public sealed class RegexComponentValidator : IComponentValidator
{
    private readonly ValidationRule _validationRule;
    public const string ErrorMessageKey = "ErrorMessage";
    public const string PropertyKey = "Property";
    public const string PatternKey = "Pattern";
    
    public RegexComponentValidator(ValidationRule validationRule)
    {
        _validationRule = validationRule;
    }
    
    public ValueTask<ValidationResult[]> ValidateAsync(ComponentModel component)
    {
        IValueComponent valueComponent = component.As<IValueComponent>();
        string pattern = _validationRule.Items.GetValue<string>(PatternKey);
        
        if (!Regex.IsMatch(valueComponent.Value ?? string.Empty, pattern))
        {
            string errorMessage = _validationRule.Items.GetValue<string>(ErrorMessageKey);
            string property = _validationRule.Items.GetValue<string>(PropertyKey);
            return ValueTask.FromResult<ValidationResult[]>([new ValidationResult(errorMessage, [property])]);
        }

        return ValueTask.FromResult<ValidationResult[]>([]);
    }
}