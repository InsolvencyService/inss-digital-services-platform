using Inss.Platform.Domain.Exceptions;
using Inss.Platform.Domain.Loading;
using Inss.Platform.Domain.Primitives;
using Inss.Platform.Domain.Validation;

namespace Inss.Platform.Domain.Components;

public abstract class Component
{
    public required ComponentId Id { get; init; }

    public abstract string ViewName { get; }
    
    public LoaderList Loaders { get; init; } = [];
    
    public ValidationList Validations { get; init; } = [];
    
    public string TypeName => GetType().FullName!;

    public virtual void CopyTo(Component targetComponent)
    {
    }
    
    public T As<T>() where T : Component
    {
        if (this is T result)
        {
            return result;
        }

        throw new ComponentException($"Cannot cast to component type {typeof(T)}.");
    }
}