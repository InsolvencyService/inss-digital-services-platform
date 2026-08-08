namespace Inss.Platform.Domain.Components.Common;

public sealed class EmailComponentModel : ComponentModel, IValueComponent
{
    public required string Question { get; init; }
    
    public string? Hint { get; init; }
    
    public string? Value { get; set; }
    
    public override void CopyTo(ComponentModel targetComponent)
    {
        EmailComponentModel email = targetComponent.As<EmailComponentModel>();
        email.Value = Value;
    }
}