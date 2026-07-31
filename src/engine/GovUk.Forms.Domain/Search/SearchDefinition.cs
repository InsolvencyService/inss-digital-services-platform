// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace GovUk.Forms.Domain.Search;

public sealed class SearchDefinition
{
    public int PageSize { get; init; }
    
    public bool DisplayAsTable { get; init; }

    public SearchResultDefinition[] Results { get; init; } = [];
    
    public SearchDetailDefinition[] Details { get; init; } = [];
    
    public SearchCategory[] Categories { get; init; } = [];

    public AuthorisingBodyLookup[] AuthorisingBodies { get; init; } = [];
}