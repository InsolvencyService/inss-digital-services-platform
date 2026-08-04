using System.ComponentModel.DataAnnotations;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Extensions;

namespace Inss.Platform.Application.Validation;

public sealed class RequiredValueComponentValidator : IComponentValidator
{
    public ValueTask<ValidationResult[]> ValidateAsync(Component component)
    {
        // TODO: Implement with custom error messages etc
        IValueComponent valueComponent = component.As<IValueComponent>();

        if (string.IsNullOrWhiteSpace(valueComponent.Value))
        {
            // TODO: Sort binding and validation mechanism
            return ValueTask.FromResult<ValidationResult[]>([new ValidationResult("The field is required.", ["Components[0].Value"])]);
        }

        return ValueTask.FromResult<ValidationResult[]>([]);
    }
}