using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.PageFlow;

public abstract class PageContext
{
    public FormModel Form { get; init; }
    
    public SectionModel Section { get; init; }
    
    public PageModel CurrentPage { get; init; }
    
    public TreeNode CurrentNode { get; init; }
}

public sealed class LoadPageContext : PageContext
{
    public Dictionary<string, string?> QueryParams { get; init; } = [];
    
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

public sealed class ValidatePageContext : PageContext
{
    public List<ValidationResult> ValidationResults { get; } = [];
}

public sealed class ExecutePageContext : PageContext
{
    public int ChildNodeIndex { get; set; }
    
    public PageModel? PageBeforeChanges { get; init; }
}