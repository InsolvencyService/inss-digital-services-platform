using Inss.Platform.Domain.Exceptions;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain.Components;

public sealed class ComponentModelList : List<ComponentModel>
{
    public ComponentModel Get(ComponentId id)
    {
        ComponentModel? component = this.FirstOrDefault(p => p.Id == id);
        return component ?? throw new ComponentException($"Cannot get component for Id {id}.");
    }
}