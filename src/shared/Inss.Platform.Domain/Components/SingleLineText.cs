using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain.Components;

public sealed class SingleLineText : ComponentModel, IValueComponent
{
    public override string ViewName => $"_{nameof(SingleLineText)}";
    
    public required Content Label { get; init; }
    
    public Content? Hint { get; init; }
    
    public string? Value { get; set; }
    
    public override void CopyTo(ComponentModel targetComponent)
    {
        SingleLineText singleLineText = targetComponent.As<SingleLineText>();
        singleLineText.Value = Value;
    }
}