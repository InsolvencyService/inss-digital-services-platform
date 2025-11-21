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

    public void AddPage(PageModel page)
    {
        // Copy the page and add - clone

        // Add identifier to copy of page

        Pages = Pages.Concat([page]).ToArray();
    }

    public string GetChangeUrl(PageModel page)
    {
        //var index = Array.IndexOf(Pages, page);
        return $"{PageUrl}?id={page.Id}";
    }
}