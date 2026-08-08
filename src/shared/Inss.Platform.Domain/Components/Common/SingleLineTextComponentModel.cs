namespace Inss.Platform.Domain.Components.Common;

// TODO: Label size, is heading 

public sealed class SingleLineTextComponentModel : ComponentModel, IValueComponent
{
    public required string Question { get; init; }
    
    public string? Hint { get; init; }
    
    public string? Value { get; set; }
    
    public override void CopyTo(ComponentModel targetComponent)
    {
        SingleLineTextComponentModel singleLineText = targetComponent.As<SingleLineTextComponentModel>();
        singleLineText.Value = Value;
    }
}