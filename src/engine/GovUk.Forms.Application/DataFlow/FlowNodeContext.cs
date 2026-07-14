using System.Globalization;
using System.Net;
using GovUk.Forms.Domain;

// ReSharper disable UnusedAutoPropertyAccessor.Global - might be used pending back button review

namespace GovUk.Forms.Application.DataFlow;

public sealed class FlowNodeContext
{
    public FlowNode[] Nodes { get; init; } = [];
    
    public FlowNode CurrentNode { get; init; }
    
    public FormModel Form { get; init; }
    
    public SectionModel Section { get; init; }
    
    public PageModel CurrentPage { get; init; }

    public IDictionary<string, string?> QueryParams { get; init; } = new Dictionary<string, string?>();
    
    public PageModel? PageBeforeChanges { get; init; }

    public void AddQueryParam<T>(string key, T value)
    {
        QueryParams[key] = WebUtility.UrlEncode(value?.ToString() ?? string.Empty);
    }
    
    public T? GetQueryParam<T>(string key)
    {
        if (!QueryParams.TryGetValue(key, out string? value) || value is null)
        {
            return default;
        }

        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        object converted = Convert.ChangeType(WebUtility.UrlDecode(value), targetType, CultureInfo.InvariantCulture);
        return (T)converted;
    }
}