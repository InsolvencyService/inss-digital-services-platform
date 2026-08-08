using System.ComponentModel.DataAnnotations;

namespace Inss.Platform.Application.Validators;

public interface IComponentValidator
{
    ValueTask<ValidationResult[]> ValidateAsync(ComponentContext context);
}