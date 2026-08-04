using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain.Components.Searching;

public sealed class SearchTermComponentModel : ComponentModel, IValueComponent, IQueryParamComponent
{
    public override string ViewName => "_SearchTerm";
    
    public required Content Heading { get; init; }

    public required Content Label { get; init; }
    
    public required Content Description { get; init; }
    
    public Content? Hint { get; init; }
    
    public string? Value { get; set; }
    
    public override void CopyTo(ComponentModel targetComponent)
    {
        SearchTermComponentModel searchTerm = targetComponent.As<SearchTermComponentModel>();
        searchTerm.Value = Value;
    }
    
    public void Append(QueryParams queryParams)
    {
        queryParams.AddQueryParam("keyword", Value);
    }
}