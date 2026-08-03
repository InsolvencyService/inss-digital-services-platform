using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain.Components;

public sealed class SingleLineText : Component, IValueComponent
{
    public override string ViewName => $"_{nameof(SingleLineText)}";
    
    public required Content Label { get; init; }
    
    public Content? Hint { get; init; }
    
    public string? Value { get; set; }
}