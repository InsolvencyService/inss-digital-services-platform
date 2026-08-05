// ReSharper disable UnusedAutoPropertyAccessor.Global - binding from JSON
namespace Inss.Platform.Domain.Components.Searching;

public sealed class SearchCategory
{
    public string Label { get; init; }
    
    public string? Css { get; init; }
}