namespace Inss.Platform.Domain.Components.Common;

public sealed class PasswordComponentModel : ComponentModel, IValueComponent
{
    public override string ViewName => "_Password";
    
    public required string Question { get; init; }
    
    public string? Value { get; set; }
    
    public override void CopyTo(ComponentModel targetComponent)
    {
        PasswordComponentModel password = targetComponent.As<PasswordComponentModel>();
        password.Value = Value;
    }
}