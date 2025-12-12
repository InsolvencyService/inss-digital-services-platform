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
    
    public BaseModel GetPageByUrl(string pageUrl)
    {
        if (pageUrl == PageUrl)
        {
            return this;
        }

        foreach (SectionModel section in Sections)
        {
            if ((section.PageUrl == pageUrl || section.PageUrl + "/summary" == pageUrl) &&
                section.Id == section.Context.CurrentPageId)
            {
                return section;
            }

            foreach (BaseModel page in section.Pages)
            {
                if (page.PageUrl == pageUrl && page.Id == section.Context.CurrentPageId)
                {
                    return page;
                }

                if (page is AddAnotherModel addAnother)
                {
                    foreach (BaseModel[] items in addAnother.Items)
                    {
                        foreach (BaseModel item in items)
                        {
                            if (item.PageUrl == pageUrl && item.Id == section.Context.CurrentPageId)
                            {
                                return item;
                            }
                        }
                    }
                }
            }
        }

        throw new FormModelException($"Unable to find the page associated with {pageUrl}.");
    }

    public SectionModel GetSectionForPageId(string pageId)
    {
        foreach (SectionModel section in Sections)
        {
            foreach (BaseModel page in section.Pages)
            {
                if (page.Id == pageId)
                {
                    return section;
                }

                if (page is AddAnotherModel addAnother)
                {
                    if (addAnother.Items.Any(items => items.Any(item => item.Id == pageId)))
                    {
                        return section;
                    }
                }
            }
        }
        
        throw new FormModelException($"Unable to find the section for page {pageId}.");
    }
    
    public BaseModel GetPageById(string id)
    {
        foreach (SectionModel section in Sections)
        {
            foreach (BaseModel page in section.Pages)
            {
                if (page is AddAnotherModel addAnother)
                {
                    if (addAnother.Id == id)
                    {
                        return addAnother;
                    }

                    foreach (BaseModel[] items in addAnother.Items)
                    {
                        foreach (BaseModel item in items)
                        {
                            if (item.Id == id)
                            {
                                return item;
                            }
                        }
                    }
                }
                else
                {
                    if (page.Id == id)
                    {
                        return page;
                    }
                }
            }
        }
        
        throw new FormModelException($"Unable to find the summary list for the specified item {id}.");
    }

    public AddAnotherModel? FindAddAnother(BaseModel model)
    {
        foreach (SectionModel section in Sections)
        {
            foreach (BaseModel page in section.Pages)
            {
                if (page is AddAnotherModel addAnother)
                {
                    foreach (BaseModel[] items in addAnother.Items)
                    {
                        foreach (BaseModel item in items)
                        {
                            if (item.Id == model.Id)
                            {
                                return addAnother;
                            }
                        }
                    }
                }
            }
        }

        return null;
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
                if (section.Pages[i] is AddAnotherModel addAnother && addAnother.PageUrl != pageUrl)
                {
                    foreach (BaseModel[] items in addAnother.Items)
                    {
                        for (int j = 0; j < items.Length; j++)
                        {
                            if (items[j].PageUrl == pageUrl && items[j].Id == section.Context.CurrentPageId)
                            {
                                if (j < items.Length - 1)
                                {
                                    section.Context.CurrentPageId = items[i + 1].Id;
                                    section.Context.PreviousPageUrl = items[i].Id;
                                    return items[i + 1];
                                }
                                
                                section.Context.CurrentPageId = addAnother.Id;
                                section.Context.PreviousPageUrl = addAnother.Id;
                                return addAnother;
                            }
                        }
                    }
                }
                else
                {
                    if (section.Pages[i].PageUrl == pageUrl &&  section.Pages[i].Id == section.Context.CurrentPageId)
                    {
                        if (i < section.Pages.Length - 1)
                        {
                            section.Context.CurrentPageId = section.Pages[i + 1].Id;
                            section.Context.PreviousPageUrl = section.Pages[i].Id;
                            return section.Pages[i + 1];
                        }

                        section.Context.CurrentPageId = section.Id;
                        section.Context.PreviousPageUrl = section.Pages[i].Id;
                        return section;
                    }
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

                if (page is AddAnotherModel addAnother)
                {
                    foreach (BaseModel page2 in addAnother.Items[0]) // TODO: Assumption that only one row will exist at this stage
                    {
                        page2.PageUrl = $"{page.PageUrl}/{page2.PathName}";
                    }
                }
            }

            BaseModel firstPage = section.Pages[0];

            if (firstPage is AddAnotherModel addAnother2)
            {
                firstPage = addAnother2.Items[0][0];
            }

            section.Context.FirstPageUrl = firstPage.PageUrl;
            section.Context.CurrentPageId = firstPage.Id;
            section.Context.PreviousPageUrl = PageUrl;
        }
    }
}