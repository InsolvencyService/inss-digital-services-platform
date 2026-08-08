using Inss.Platform.Application.Validation;
using Inss.Platform.Domain.Validation;

namespace Inss.Platform.Component.Builders;

public static class ComponentModelBuilderExtensions
{
    extension(ComponentModelBuilder componentModelBuilder)
    {
        public ComponentModelBuilder WithRequiredValidator(string errorMessage)
        {
            ValidationRule rule = new()
            {
                ValidatorType = typeof(RequiredValueComponentValidator),
                Items = new ValidationRuleItemList
                {
                    [RequiredValueComponentValidator.ErrorMessageKey] = errorMessage,
                    [RequiredValueComponentValidator.PropertyKey] = $"Components[{componentModelBuilder.ComponentIndex}].Value"
                }
            };
            componentModelBuilder.CurrentComponent.Validations = [..componentModelBuilder.CurrentComponent.Validations, rule];
            return componentModelBuilder;
        }
        
        public ComponentModelBuilder WithMaxLengthValidator(int maxLength, string errorMessage)
        {
            ValidationRule rule = new()
            {
                ValidatorType = typeof(MaxLengthComponentValidator),
                Items = new ValidationRuleItemList
                {
                    [MaxLengthComponentValidator.ErrorMessageKey] = errorMessage,
                    [MaxLengthComponentValidator.PropertyKey] = $"Components[{componentModelBuilder.ComponentIndex}].Value",
                    [MaxLengthComponentValidator.MaxLengthKey] = $"{maxLength}"
                }
            };
            componentModelBuilder.CurrentComponent.Validations = [..componentModelBuilder.CurrentComponent.Validations, rule];
            return componentModelBuilder;
        }
        
        public ComponentModelBuilder WithRegexValidator(string pattern, string errorMessage)
        {
            ValidationRule rule = new()
            {
                ValidatorType = typeof(RegexComponentValidator),
                Items = new ValidationRuleItemList
                {
                    [RegexComponentValidator.ErrorMessageKey] = errorMessage,
                    [RegexComponentValidator.PropertyKey] = $"Components[{componentModelBuilder.ComponentIndex}].Value",
                    [RegexComponentValidator.PatternKey] = pattern
                }
            };
            componentModelBuilder.CurrentComponent.Validations = [..componentModelBuilder.CurrentComponent.Validations, rule];
            return componentModelBuilder;
        }
        
        public ComponentModelBuilder WithEmailValidator(string errorMessage)
        {
            ValidationRule rule = new()
            {
                ValidatorType = typeof(EmailComponentValidator),
                Items = new ValidationRuleItemList
                {
                    [EmailComponentValidator.ErrorMessageKey] = errorMessage,
                    [EmailComponentValidator.PropertyKey] = $"Components[{componentModelBuilder.ComponentIndex}].Value"
                }
            };
            componentModelBuilder.CurrentComponent.Validations = [..componentModelBuilder.CurrentComponent.Validations, rule];
            return componentModelBuilder;
        }
    }
}