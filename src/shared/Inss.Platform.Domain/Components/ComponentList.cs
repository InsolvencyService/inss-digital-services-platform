using Inss.Platform.Domain.Exceptions;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain.Components;

public sealed class ComponentList : List<Component>
{
    public Component Get(ComponentId id)
    {
        Component? component = this.FirstOrDefault(p => p.Id == id);
        return component ?? throw new ComponentException($"Cannot get component for Id {id}.");
    }
}