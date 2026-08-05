namespace Inss.Platform.Domain.Components.Searching;

public sealed class SearchDefinition
{
    public int PageSize { get; init; }
    
    public bool DisplayAsTable { get; init; }

    public SearchResultDefinition[] Results { get; init; } = [];
    
    public SearchDetailDefinition[] Details { get; init; } = [];
    
    public SearchCategory[] Categories { get; init; } = [];
}