using GovUk.Forms.Domain.Exceptions;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Domain;

public sealed class PageModelList : List<PageModel>
{
    public PageModel? FindPage(ContentPath path)
    {
        return GetAllPathPages().FirstOrDefault(page => page.Path == path);
    }
    
    public PageModel GetPage(ContentPath path)
    {
        PageModel? page = GetAllPathPages().FirstOrDefault(page => page.Path == path);
        return page ?? throw new ModelException($"Unable to find page for path {path}.");
    }

    public void ResetDownstream(PageModel currentPage)
    {
        int pageIndex = IndexOf(currentPage);

        if (pageIndex > -1)
        {
            for (int i = pageIndex + 1; i < Count; i++)
            {
                PageModel page = this[i];
                page.ClearValues();
            }
        }
    }
    
    public PageModelList GetCompletedPages()
    {
        PageModelList savedPageList = [];
        
        foreach (PageModel page in this)
        {
            if (page is GroupPageModel groupPage)
            {
                foreach (PageModel subPage in groupPage.SavablePages)
                {
                    if (subPage.CompletedDate is not null)
                    {
                        savedPageList.Add(subPage);
                    }
                }
            }
            else
            {
                if (page.CompletedDate is not null)
                {
                    savedPageList.Add(page);
                }
            }
        }

        return savedPageList;
    }

    public PageModelList GetAllPathPages()
    {
        PageModelList pages = [];

        foreach (PageModel page in this)
        {
            if (page is GroupPageModel groupPage)
            {
                foreach (PageModel subPage in groupPage.Pages)
                {
                    pages.Add(subPage);
                }
            }
            else
            {
                pages.Add(page);
            }
        }
        
        return pages;
    }

    public T GetFirstOf<T>() where T : PageModel
    {
        foreach (PageModel page in this)
        {
            if (page is GroupPageModel groupPage)
            {
                foreach (PageModel subPage in groupPage.Pages)
                {
                    if (subPage is T subPageAsType)
                    {
                        return subPageAsType;
                    }
                }
            }
            
            if (page is T pageAsType)
            {
                return pageAsType;
            }
        }
        
        throw new ModelException($"Unable to find page of type {typeof(T)}.");
    }

    public TGroup GetGroup<TGroup>(GroupId groupId) where TGroup : GroupPageModel
    {
        foreach (PageModel page in this)
        {
            if (page is TGroup groupPage)
            {
                if (page.MetaData.Group == groupId)
                {
                    return groupPage;
                }
            }
        }
        
        throw new ModelException($"Unable to find group page of type {typeof(TGroup)}.");   
    }

    public T GetAtIndex<T>(int index) where T : PageModel
    {
        List<T> typeMatches = [];
        
        foreach (PageModel page in this)
        {
            if (page is GroupPageModel groupPage)
            {
                foreach (PageModel subPage in groupPage.Pages)
                {
                    if (subPage is T subPageAsType)
                    {
                        typeMatches.Add(subPageAsType);
                    }
                }
            }
            
            if (page is T pageAsType)
            {
                typeMatches.Add(pageAsType);
            }
        }

        if (typeMatches.Count == 0 || typeMatches.Count < index)
        {
            throw new ModelException($"Unable to find page of type {typeof(T)} at index {index}.");    
        }

        return typeMatches[index];
    }
    
    public bool RemoveMatchingPages(ContentId[] identifiers)
    {
        List<PageModel> matchingPages = [];

        for (int i = 0; i < Count; i++)
        {
            if (identifiers.Any(id => this[i].Id == id))
            {
                matchingPages.Add(this[i]);
            }
        }

        foreach (PageModel matchingPage in matchingPages)
        {
            Remove(matchingPage);
        }

        return matchingPages.Count > 0;
    }
}