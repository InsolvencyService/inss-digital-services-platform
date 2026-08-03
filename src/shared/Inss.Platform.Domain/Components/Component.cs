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
}