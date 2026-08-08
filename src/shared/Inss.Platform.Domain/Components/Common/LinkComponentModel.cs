namespace Inss.Platform.Domain.Components.Common;

public sealed class LinkComponentModel : ComponentModel
{
    public override string ViewName => "_Link";
    
    public required string Label { get; init; }
    
    public required string Url { get; init; }
}