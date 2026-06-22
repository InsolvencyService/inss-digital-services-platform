namespace GovUk.Forms.Infrastructure.Options;

public sealed class SearchPersonOptions
{
    public string? Endpoint { get; init; }

    public string? IndexName { get; init; } = string.Empty;

    public string? ApiKey { get; init; } = string.Empty;

    public string? ApiVersion { get; init; } = string.Empty;

    //public int PageSize { get; init; }

    //public string? DisplayMode { get; init; }

    //public List<SearchColumnOptions> Columns { get; set; } = new List<SearchColumnOptions>();
}


//internal sealed class SearchColumnOptions
//{
//    public string FieldName { get; set; } = string.Empty;
//    public string Header { get; set; } = string.Empty;
//    public int Order { get; set; }
//    public bool IsVisible { get; set; }
//    public string CssClass { get; set; } = string.Empty;
//}
