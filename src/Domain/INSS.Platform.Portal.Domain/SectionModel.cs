namespace INSS.Platform.Portal.Domain;

public sealed class SectionModel : BaseModel
{
    public string Name { get; set; } = string.Empty;

    public bool IsComplete { get; set; }

    public PageModel[] Pages { get; set; } = [];
    
    public string PathName { get; init; } = "Section";
    
    public string PageUrl { get; set; } = string.Empty;
    
    public string PreviousPageUrl { get; set; } = string.Empty;

    public void AddPage(PageModel page)
    {
        page.PageUrl = $"{PageUrl}/{page.PathName}";
        Pages = Pages.Concat([page]).ToArray();
    }
    
    public PageModel? GetNextPage(string pageUrl)
    {
        for (int i = 0; i < Pages.Length; i++)
        {
            if (Pages[i].PageUrl == pageUrl && i < Pages.Length - 1)
            {
                return Pages[i + 1];
            }
        }

        return null;
    }
    
    public bool IsLastPageInSection(PageModel page)
    {
        return Pages.Last().PageUrl == page.PageUrl;   
    }
    public PageModel? GetPreviousPage(string pageUrl)
    {
        for (int i = 0; i < Pages.Length; i++)
        {
            if (Pages[i].PageUrl == pageUrl && i > 0)
            {
                return Pages[i - 1];
            }
        }

        return null;
    }

    public PageModel InsertRemovePage(PageModel page, string id)
    {
        List<PageModel> pageList = Pages.ToList();
        ConfirmModel confirmModel = new() { Id = id, PageUrl = $"{page.PageUrl}/confirm" };
        pageList.Insert(pageList.IndexOf(page) + 1, confirmModel);
        Pages = pageList.ToArray();
        return confirmModel;
    }

}