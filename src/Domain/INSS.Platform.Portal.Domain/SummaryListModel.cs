namespace INSS.Platform.Portal.Domain;

public class SummaryListModel : PageModel
{
    public SummaryListModel()
    {
        PathName = "summary-list";
        Title = "Summary List";
        Controller = "SummaryList";
    }

    public PageModel[] Pages { get; set; } = [];

    public bool Reload { get; set; }

    public void AddPage(PageModel page)
    {
        // Copy the page and add - clone

        // Add identifier to copy of page

        Pages = Pages.Concat([page]).ToArray();
    }

    public void RemovePage(PageModel page)
    {
        List<PageModel> pageList = Pages.ToList();

        pageList.Remove(page);
        Pages = pageList.ToArray();
    }

    public string GetChangeUrl(PageModel page)
    {
        return $"{PageUrl}/change/?id={page.Id}";
    }

    public string GetRemoveUrl(PageModel page)
    {
        return $"{PageUrl}/remove/?id={page.Id}";
    }
}