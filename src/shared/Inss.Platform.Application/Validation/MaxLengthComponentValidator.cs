using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Extensions;

namespace Inss.Platform.Application.Validation;

public sealed class MaxLengthComponentValidator : IComponentValidator
{
    private readonly int _maxLength;

    public MaxLengthComponentValidator(int maxLength)
    {
        _maxLength = maxLength;
    }
    public ValueTask ValidateAsync(Component component)
    {
        IValueComponent valueComponent = component.As<IValueComponent>();

        if (valueComponent.Value?.Length > _maxLength)
        {
            Console.WriteLine("Value too long!");
        }

        return ValueTask.CompletedTask;
    }
}