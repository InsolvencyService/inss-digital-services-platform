using Inss.Platform.Domain.Exceptions;
using Inss.Platform.Domain.Loading;
using Inss.Platform.Domain.Primitives;
using Inss.Platform.Domain.Validation;

namespace Inss.Platform.Domain.Components;

public abstract class ComponentModel
{
    public required ComponentId Id { get; init; }
    
    public required PagePath AssociatedPagePath { get; init; }

    public virtual string ViewName => $"_{GetType().Name.Replace("ComponentModel", string.Empty)}";
    
    public LoaderList Loaders { get; set; } = [];
    
    public ValidationList Validations { get; set; } = [];
    
    public string TypeName => GetType().FullName!;

    public ComponentTypes ComponentType { get; init; } = ComponentTypes.Bindable;
    
    public virtual void CopyTo(ComponentModel targetComponent)
    {
    }
    
    public T As<T>() where T : ComponentModel
    {
        if (this is T result)
        {
            return result;
        }

        throw new ComponentException($"Cannot cast to component type {typeof(T)}.");
    }
}