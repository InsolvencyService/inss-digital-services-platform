using System.ComponentModel.DataAnnotations;

namespace Inss.Platform.Application.Validation;

public interface IComponentValidator
{
    ValueTask<ValidationResult[]> ValidateAsync(ComponentContext context);
}