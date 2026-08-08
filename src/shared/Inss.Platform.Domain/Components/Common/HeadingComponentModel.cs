namespace Inss.Platform.Domain.Components.Common;

public sealed class HeadingComponentModel : ComponentModel
{
    public override string ViewName => "_Heading";
    
    public required string Heading { get; init; }
}