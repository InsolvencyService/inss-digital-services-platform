namespace Inss.Platform.Domain.Components.Common;

public sealed class LinkComponentModel : ComponentModel
{
    public required string Label { get; init; }
    
    public required string Url { get; init; }
}