namespace INSS.Platform.Portal.Domain;

public sealed class SummaryListModel : BaseModel
{
    public SummaryListModel()
    {
        PathName = "summary-list";
        Name = "Summary List";
    }

    public BaseModel[] Items { get; set; } = [];

    public string GetChangeUrl(BaseModel item)
    {
        return $"{PageUrl}/change/?itemId={item.Id}";
    }
    
    public string GetRemoveUrl(BaseModel item)
    {
        return $"{PageUrl}/remove/?itemId={item.Id}";
    }
}