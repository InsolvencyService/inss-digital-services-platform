using INSS.Platform.Portal.Domain.Exceptions;

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
        if (pageUrl == PageUrl)
        {
            return this;
        }

        foreach (SectionModel section in Sections)
        {
            if (section.PageUrl == pageUrl || section.PageUrl + "/summary" == pageUrl)
            {
                return section;
            }

            foreach (BaseModel page in section.Pages)
            {
                if (page.PageUrl == pageUrl)
                {
                    return page;
                }
            }
        }

        throw new FormModelException($"Unable to find the page associated with {pageUrl}.");
    }

    public SummaryListModel FindSummaryList(string itemId)
    {
        foreach (SectionModel section in Sections)
        {
            foreach (BaseModel page in section.Pages)
            {
                if (page is SummaryListModel summaryList && summaryList.Items.Any(i => i.Id == itemId))
                {
                    return summaryList;
                }
            }
        }

        throw new FormModelException($"Unable to find the summary list for the specified item {itemId}.");
    }
    
    public BaseModel FindPageBefore(BaseModel currentPage)
    {
        foreach (SectionModel section in Sections)
        {
            for (int i = 0; i < section.Pages.Length; i++)
            {
                if (section.Pages[i].Id == currentPage.Id && i > 0)
                {
                    return section.Pages[i - 1];
                }
            }
        }

        throw new FormModelException($"Unable to find the page before {currentPage.PageUrl}.");
    }
    
    public BaseModel GetNextPageAfter(string pageUrl)
    {
        foreach (SectionModel section in Sections)
        {
            if (section.PageUrl == pageUrl)
            {
                return this;
            }

            for (int i = 0; i < section.Pages.Length; i++)
            {
                if (section.Pages[i].PageUrl == pageUrl)
                {
                    if (i < section.Pages.Length - 1)
                    {
                        return section.Pages[i + 1];
                    }

                    return section;
                }
            }
        }

        return this;
    }
    
    public void Initialize()
    {
        PageUrl = $"/{PathName}";
        
        foreach (SectionModel section in Sections)
        {
            section.PageUrl = $"{PageUrl}/{section.PathName}";

            foreach (BaseModel page in section.Pages)
            {
                page.PageUrl = $"{section.PageUrl}/{page.PathName}";
            }
        }
    }
}