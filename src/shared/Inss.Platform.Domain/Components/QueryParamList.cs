using System.Net;

namespace Inss.Platform.Domain.Components;

public sealed class QueryParamList : Dictionary<string, string?>
{
    public void AddQueryParam<T>(string key, T value)
    {
        this[key] = WebUtility.UrlEncode(value?.ToString() ?? string.Empty);
    }
}