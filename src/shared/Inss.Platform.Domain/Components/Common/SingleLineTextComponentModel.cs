using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain.Components.Common;

public sealed class SingleLineTextComponentModel : ComponentModel, IValueComponent
{
    public override string ViewName => "_SingleLineText";
    
    public required Content Label { get; init; }
    
    public Content? Hint { get; init; }
    
    public string? Value { get; set; }
    
    public override void CopyTo(ComponentModel targetComponent)
    {
        SingleLineTextComponentModel singleLineText = targetComponent.As<SingleLineTextComponentModel>();
        singleLineText.Value = Value;
    }
}