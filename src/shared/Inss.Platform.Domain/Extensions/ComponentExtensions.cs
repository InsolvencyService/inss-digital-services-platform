using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Exceptions;

namespace Inss.Platform.Domain.Extensions;

public static class ComponentExtensions
{
    extension(Component component)
    {
        public T As<T>()
        {
            return component is not T valueComponent 
                ? throw new ComponentException($"Unable to convert component to type {nameof(T)}.") 
                : valueComponent;
        }
    }
}