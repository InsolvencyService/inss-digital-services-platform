using Inss.Platform.Domain.Components;

namespace Inss.Platform.Application.Validation;

public interface IComponentValidator
{
    ValueTask ValidateAsync(Component component);
}