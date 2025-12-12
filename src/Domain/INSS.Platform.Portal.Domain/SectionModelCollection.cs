using INSS.Platform.Portal.Domain.Exceptions;

namespace INSS.Platform.Portal.Domain;

public sealed class SectionModelCollection : List<SectionModel>
{
    public SectionModel GetSectionByUrl(string pageUrl)
    {
        foreach (SectionModel section in this.Where(section => section.PageUrl == pageUrl))
        {
            return section;
        }

        throw new FormModelException($"Unable to find the section for url {pageUrl}.");
    }

    public AddAnotherModel GetAddAnotherFor(BaseModel model)
    {
        foreach (SectionModel section in this)
        {
            foreach (BaseModel page in section.Pages)
            {
                if (page is AddAnotherModel addAnother)
                {
                    BaseModel? existingPage = addAnother.Items.FindExistingFor(model.Id);

                    if (existingPage is not null)
                    {
                        return addAnother;
                    }
                }
            }
        }

        throw new FormModelException($"Unable to find the add another model associated to item {model.Id}.");
    }
    
    public BaseModel? GetCurrentPageFor(BaseModel model)
    {
        foreach (SectionModel section in this)
        {
            if (section.Id == model.Id &&  section.PageUrl == model.PageUrl)
            {
                return section;
            }
            
            foreach (BaseModel page in section.Pages)
            {
                if (page is AddAnotherModel addAnother)
                {
                    if (addAnother.Id == model.Id &&  addAnother.PageUrl == model.PageUrl)
                    {
                        return addAnother;
                    }

                    BaseModel? existingPage = addAnother.Items.FindExistingFor(model);

                    if (existingPage is not null)
                    {
                        return existingPage;
                    }
                }
                else
                {
                    if (page.Id == model.Id &&   page.PageUrl == model.PageUrl)
                    {
                        return page;
                    }
                }
            }
        }

        return null;
    }
    
    public BaseModel? FindPage(string pageId)
    {
        foreach (SectionModel section in this)
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

                    BaseModel? existingPage = addAnother.Items.FindExistingFor(pageId);

                    if (existingPage is not null)
                    {
                        return existingPage;
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

        return null;
    }
    
    public BaseModel? FindNextPageAfter(BaseModel model)
    {
        foreach (SectionModel section in this)
        {
            if (section.Id == model.Id)
            {
                return null;
            }

            for (int i = 0; i < section.Pages.Count; i++)
            {
                if (section.Pages[i] is AddAnotherModel addAnother && addAnother.Id != model.Id)
                {
                    BaseModel? nextPage = addAnother.Items.GetNextPageAfter(model, addAnother);

                    if (nextPage is not null)
                    {
                        return  nextPage;
                    }
                }
                else
                {
                    if (section.Pages[i].Id == model.Id)
                    {
                        if (i < section.Pages.Count - 1)
                        {
                            return section.Pages[i + 1];
                        }

                        return section;
                    }
                }
            }
        }
        
        return null;
    }
}