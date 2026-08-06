using System.Globalization;
using System.Net;
using Inss.Platform.Domain.Exceptions;

namespace Inss.Platform.Domain.Components;

public sealed class QueryParamList : Dictionary<string, string?>
{
    public void AddQueryParam<T>(string key, T value)
    {
        this[key] = WebUtility.UrlEncode(value?.ToString() ?? string.Empty);
    }
    
    public T? FindQueryParam<T>(string key)
    {
        if (!this.TryGetValue(key, out string? value) || value is null)
        {
            return default;
        }

        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        object converted = Convert.ChangeType(WebUtility.UrlDecode(value), targetType, CultureInfo.InvariantCulture);
        return (T)converted;
    }
    
    public T GetQueryParam<T>(string key)
    {
        if (!this.TryGetValue(key, out string? value) || value is null)
        {
            throw new ComponentException($"No {key} query param found.");
        }

        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        object converted = Convert.ChangeType(WebUtility.UrlDecode(value), targetType, CultureInfo.InvariantCulture);
        return (T)converted;
    }

    public string? BuildQueryParams()
    {
        List<string> parameters = [];

        foreach (var item in this)
        {
            parameters.Add($"{item.Key}={item.Value}");
        }

        return parameters.Count > 0 ? $"?{string.Join('&', parameters)}" : null;
    }
}