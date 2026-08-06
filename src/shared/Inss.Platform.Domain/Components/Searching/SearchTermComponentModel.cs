namespace Inss.Platform.Domain.Components.Searching;

public sealed class SearchTermComponentModel : ComponentModel, IValueComponent, IQueryParamComponent
{
    public override string ViewName => "_SearchTerm";
    
    public required string Heading { get; init; }

    public required string Label { get; init; }
    
    public required string Description { get; init; }
    
    public string? Hint { get; init; }
    
    public string? Value { get; set; }
    
    public override void CopyTo(ComponentModel targetComponent)
    {
        SearchTermComponentModel searchTerm = targetComponent.As<SearchTermComponentModel>();
        searchTerm.Value = Value;
    }
    
    public void Append(QueryParamList queryParams)
    {
        queryParams.AddQueryParam("keyword", Value);
    }
}