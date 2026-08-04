using System.Net;

namespace Inss.Platform.Domain.Components;

public interface IValueComponent
{
    string? Value { get; set; }
}

public interface IQueryParamComponent
{
    void Append(QueryParams queryParams);
}


public sealed class QueryParams : Dictionary<string, string?>
{
    public void AddQueryParam<T>(string key, T value)
    {
        this[key] = WebUtility.UrlEncode(value?.ToString() ?? string.Empty);
    }
}