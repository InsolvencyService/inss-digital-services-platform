using System.Globalization;
using System.Net;

namespace Inss.Platform.Domain.Components;

public sealed class QueryParamList : Dictionary<string, string?>
{
    public void AddQueryParam<T>(string key, T value)
    {
        this[key] = WebUtility.UrlEncode(value?.ToString() ?? string.Empty);
    }
    
    public T? GetQueryParam<T>(string key)
    {
        if (!this.TryGetValue(key, out string? value) || value is null)
        {
            return default;
        }

        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        object converted = Convert.ChangeType(WebUtility.UrlDecode(value), targetType, CultureInfo.InvariantCulture);
        return (T)converted;
    }
}