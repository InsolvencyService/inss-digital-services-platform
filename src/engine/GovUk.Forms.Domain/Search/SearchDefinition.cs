// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace GovUk.Forms.Domain.Search;

public sealed class SearchDefinition
{
    public int PageSize { get; init; }
    
    public bool DisplayAsTable { get; init; }

    public SearchDefinitionField[] Fields { get; init; } = [];

    public SearchCategory[] Categories { get; init; } = [];
}