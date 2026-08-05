using Inss.Platform.Domain.Exceptions;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain.Components;

public sealed class ComponentModelList : List<ComponentModel>
{
    public ComponentModel GetComponent(ComponentId id)
    {
        ComponentModel? component = this.FirstOrDefault(p => p.Id == id);
        return component ?? throw new ComponentException($"Cannot get component for Id {id}.");
    }

    public TComponent GetFirstOf<TComponent>() where TComponent : ComponentModel
    {
        foreach (ComponentModel component in this)
        {
            if (component is TComponent componentAsType)
            {
                return componentAsType;
            }
        }
        
        throw new ComponentException($"Unable to find component of type {typeof(TComponent).Name}.");
    }
    
    public bool HasComponent<TComponent>() where TComponent : ComponentModel
    {
        return this.Any(t => t is TComponent);
    }
}