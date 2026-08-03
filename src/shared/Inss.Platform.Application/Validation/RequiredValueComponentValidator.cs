using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Extensions;

namespace Inss.Platform.Application.Validation;

public sealed class RequiredValueComponentValidator : IComponentValidator
{
    public ValueTask ValidateAsync(Component component)
    {
        IValueComponent valueComponent = component.As<IValueComponent>();

        if (string.IsNullOrWhiteSpace(valueComponent.Value))
        {
            Console.WriteLine("Value is missing!");
        }

        return ValueTask.CompletedTask;
    }
}