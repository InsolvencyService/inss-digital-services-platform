using INSS.Platform.Portal.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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

    [ValidateNever]
    public FormContext Context { get; } = new();

    public BaseModel GetCurrentPage()
    {
        string pageId = Context.CurrentPageId;

        if (Id == pageId)
        {
            return this;
        }
        
        foreach (SectionModel section in Sections)
        {
            if (section.Id == pageId)
            {
                return section;
            }
            
            foreach (BaseModel page in section.Pages)
            {
                if (page is AddAnotherModel addAnother)
                {
                    if (addAnother.Id == pageId)
                    {
                        return addAnother;
                    }

                    foreach (BaseModel[] items in addAnother.Items)
                    {
                        foreach (BaseModel item in items)
                        {
                            if (item.Id == pageId)
                            {
                                return item;
                            }
                        }
                    }
                }
                else
                {
                    if (page.Id == pageId)
                    {
                        return page;
                    }
                }
            }
        }

        return this;
    }
    
    public BaseModel GetPageByUrl(string pageUrl)
    {
        if (pageUrl == PageUrl)
        {
            return this;
        }

        foreach (SectionModel section in Sections)
        {
            if ((section.PageUrl == pageUrl || section.PageUrl + "/summary" == pageUrl) &&
                section.Id == Context.CurrentPageId)
            {
                return section;
            }

            foreach (BaseModel page in section.Pages)
            {
                if (page.PageUrl == pageUrl && page.Id == Context.CurrentPageId)
                {
                    return page;
                }

                if (page is AddAnotherModel addAnother)
                {
                    foreach (BaseModel[] items in addAnother.Items)
                    {
                        foreach (BaseModel item in items)
                        {
                            if (item.PageUrl == pageUrl && item.Id == Context.CurrentPageId)
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
    
    public SectionModel GetSectionByUrl(string pageUrl)
    {
        foreach (SectionModel section in Sections)
        {
            if (section.PageUrl == pageUrl)
            {
                return section;
            }
        }
        
        throw new FormModelException($"Unable to find the section for url {pageUrl}.");
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
    
    public BaseModel GetNextPageAfter(string pageId)
    {
        foreach (SectionModel section in Sections)
        {
            if (section.Id == pageId)
            {
                return this;
            }

            for (int i = 0; i < section.Pages.Length; i++)
            {
                if (section.Pages[i] is AddAnotherModel addAnother && addAnother.Id != pageId)
                {
                    foreach (BaseModel[] items in addAnother.Items)
                    {
                        for (int j = 0; j < items.Length; j++)
                        {
                            if (items[j].Id == pageId)
                            {
                                if (j < items.Length - 1)
                                {
                                    Context.CurrentPageId = items[i + 1].Id;
                                    Context.PreviousPageUrl = items[i].Id;
                                    return items[i + 1];
                                }
                                
                                Context.CurrentPageId = addAnother.Id;
                                Context.PreviousPageUrl = addAnother.Id;
                                return addAnother;
                            }
                        }
                    }
                }
                else
                {
                    if (section.Pages[i].Id == pageId)
                    {
                        if (i < section.Pages.Length - 1)
                        {
                            Context.CurrentPageId = section.Pages[i + 1].Id;
                            Context.PreviousPageUrl = section.Pages[i].Id;
                            return section.Pages[i + 1];
                        }

                        Context.CurrentPageId = section.Id;
                        Context.PreviousPageUrl = section.Pages[i].Id;
                        return section;
                    }
                }
            }
        }

        Context.CurrentPageId = Id;
        Context.PreviousPageUrl = null;
        
        return this;
    }
    
    public void Initialize()
    {
        PageUrl = $"/{PathName}";

        Context.CurrentPageId = Id;
        
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

            //section.Context.FirstPageUrl = firstPage.PageUrl;
            //section.Context.CurrentPageId = firstPage.Id;
            //section.Context.PreviousPageUrl = PageUrl;
        }
    }
}