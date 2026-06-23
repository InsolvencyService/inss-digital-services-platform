using System.Globalization;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow;

public sealed class FlowNodeContext
{
    public FlowNode[] Nodes { get; init; } = [];
    
    public FlowNode CurrentNode { get; init; }
    
    public FormModel Form { get; init; }
    
    public SectionModel Section { get; init; }
    
    public PageModel CurrentPage { get; init; }

    public Dictionary<string, string?> QueryParams { get; init; } = [];

    public ContentPath? RefererPath { get; init; }

    public T? GetQueryParam<T>(string key)
    {
        if (!QueryParams.TryGetValue(key, out string? value) || value is null)
        {
            return default;
        }

        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        object converted = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        return (T)converted;
    }
}