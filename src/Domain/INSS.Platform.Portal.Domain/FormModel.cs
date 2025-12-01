namespace INSS.Platform.Portal.Domain;

public class FormModel : BaseModel
{
    public FormModel()
    {
        PathName = "tasks";
        Name = "Tasks";
    }

    public SectionModel[] Sections { get; init; } = [];

    public bool CanSubmit => Sections.All(s => s.IsComplete);
    
    public BaseModel FindPage(string pageUrl)
    {
        if (pageUrl == PageUrl) return this;

        foreach (var section in Sections)
        {
            if (section.PageUrl == pageUrl || section.PageUrl + "/summary" == pageUrl)
            {
                return section;
            }

            foreach (var page in section.Pages)
            {
                if (page.PageUrl == pageUrl)
                {
                    return page;
                }
            }
        }

        return this; // TODO: Handle?
    }

    public SummaryListModel FindSummaryList(string itemId)
    {
        foreach (var section in Sections)
        {
            foreach (var page in section.Pages)
            {
                if (page is SummaryListModel summaryList && summaryList.Items.Any(i => i.Id == itemId))
                {
                    return summaryList;
                }
            }
        }

        throw new InvalidOperationException("Shouldn't get here");
    }
    
    public BaseModel FindPageBefore(BaseModel currentPage)
    {
        foreach (var section in Sections)
        {
            for (int i = 0; i < section.Pages.Length; i++)
            {
                if (section.Pages[i].Id == currentPage.Id && i > 0)
                {
                    return section.Pages[i - 1];
                }
            }
        }

        throw new InvalidOperationException("Shouldn't get here");
    }
    
    public BaseModel GetNextPageAfter(string pageUrl)
    {
        foreach (var section in Sections)
        {
            if (section.PageUrl == pageUrl)
            {
                return this; // We have completed the section so return to the task list
            }

            // TODO: Need to integrate with state machine to decide on next page

            for (int i = 0; i < section.Pages.Length; i++)
            {
                if (section.Pages[i].PageUrl == pageUrl)
                {
                    if (i < section.Pages.Length - 1)
                    {
                        return section.Pages[i + 1];
                    }

                    return section; // TODO: return the summary for the section
                }
            }
        }

        return this; // TODO: Is this what we want?
    }
    
    public void Initialize()
    {
        PageUrl = $"/{PathName}";
        
        foreach (var section in Sections)
        {
            section.PageUrl = $"{PageUrl}/{section.PathName}";

            foreach (var page in section.Pages)
            {
                page.PageUrl = $"{section.PageUrl}/{page.PathName}";
            }
        }
    }
}