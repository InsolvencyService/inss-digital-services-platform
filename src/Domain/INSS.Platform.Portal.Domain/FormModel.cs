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

    public bool CanSubmit => Sections.All(s => s.Progress == SectionProgressType.Completed);
    
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

                if (page is AddAnotherModel addAnother)
                {
                    if (addAnother.CurrentAction == AddAnotherActionMode.Add)
                    {
                        foreach (BaseModel item in addAnother.Items.Last())
                        {
                            if (item.PageUrl == pageUrl)
                            {
                                return item;
                            }
                        }
                    }
                    else
                    {
                        foreach (BaseModel[] items in addAnother.Items)
                        {
                            foreach (BaseModel item in items)
                            {
                                if (item.PageUrl == pageUrl && addAnother.CurrentEditId == item.Id)
                                {
                                    return item;
                                }
                            }
                        }
                    }
                    // foreach (BaseModel page2 in addAnother.Items)
                    // {
                    //     if (page2.PageUrl == pageUrl)
                    //     {
                    //         // if (addAnother.Items.Count > 0 && addAnother.Items[0].Length == addAnother.Pages.Length)
                    //         // {
                    //         //     return addAnother;
                    //         // }
                    //         return page2;
                    //     }
                    // }
                }
            }
        }

        throw new FormModelException($"Unable to find the page associated with {pageUrl}.");
    }

    public BaseModel FindPageById(string id)
    {
        foreach (SectionModel section in Sections)
        {
            foreach (BaseModel page in section.Pages)
            {
                if (page.Id == id)
                {
                    return page;
                }

                if (page is AddAnotherModel addAnother)
                {
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
                    
                    // foreach (BaseModel page2 in addAnother.Pages)
                    // {
                    //     if (page2.Id == id)
                    //     {
                    //         return  page2;
                    //     }
                    // }
                }
            }
        }
        
        throw new FormModelException($"Unable to find the summary list for the specified item {id}.");
    }
    
    public BaseModel FindPageById2(string id)
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
                                return  item;
                            }
                        }
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
                    // foreach (BaseModel page2 in addAnother.Pages)
                    // {
                    //     if (page2.Id == model.Id)
                    //     {
                    //         return addAnother;
                    //     }
                    // }

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

                if (page is AddAnotherModel addAnother)
                {
                    foreach (BaseModel page2 in addAnother.Items[0]) // TODO: Assumption that only one row will exist at this stage
                    {
                        page2.PageUrl = $"{page.PageUrl}/{page2.PathName}";
                    }
                }
            }
        }
    }
}