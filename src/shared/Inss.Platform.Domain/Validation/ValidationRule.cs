namespace Inss.Platform.Domain.Validation;

public sealed class ValidationRule
{
    public required Type ValidatorType { get; init; }
    
    public ValidationRuleItemList Items { get; init; } = [];
}