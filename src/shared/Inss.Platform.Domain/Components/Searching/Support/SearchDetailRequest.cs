namespace Inss.Platform.Domain.Components.Searching.Support;

public sealed class SearchDetailRequest
{
    public required string KeyField { get; init; }
    
    public required string KeyValue { get; init; }
}