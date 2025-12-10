namespace INSS.Platform.Portal.Domain;

public sealed class SummaryListModel : BaseModel
{
    public SummaryListModel()
    {
        PathName = "summary-list";
        Name = "Summary List";
    }

    public BaseModel[] Items { get; set; } = [];

    public string RemoveQuestionText { get; set; }
    
    public string GetChangeUrl(BaseModel item)
    {
        return $"{PageUrl}/change/?itemId={item.Id}";
    }
    
    public string GetRemoveUrl(BaseModel item)
    {
        return $"{PageUrl}/remove/?itemId={item.Id}";
    }
}

public sealed class AddAnotherModel : BaseModel
{
    public AddAnotherModel()
    {
        PathName = "add-another";
        Name = "Add Another";
    }
    
    public BaseModel[] Pages { get; set; } = [];

    public List<BaseModel[]> Items { get; set; } = [];

    public BaseModel GetNextPage(BaseModel currentPage)
    {
        for (int i = 0; i < Pages.Length; i++)
        {
            if (Pages[i].Id == currentPage.Id && i < Pages.Length - 1)
            {
                return Pages[i + 1];
            }
        }

        return this;
    }
    
    public BaseModel GetFirstPage()
    {
        return Pages.First();
    }
    
    public string GetChangeUrl(BaseModel item)
    {
        return $"{PageUrl}/change/?itemId={item.Id}";
    }
    
    public string GetRemoveUrl(BaseModel item)
    {
        return $"{PageUrl}/remove/?itemId={item.Id}";
    }
}