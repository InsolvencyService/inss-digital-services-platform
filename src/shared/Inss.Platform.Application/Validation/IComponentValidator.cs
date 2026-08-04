using System.ComponentModel.DataAnnotations;
using Inss.Platform.Domain.Components;

namespace Inss.Platform.Application.Validation;

public interface IComponentValidator
{
    ValueTask<ValidationResult[]> ValidateAsync(Component component);
}