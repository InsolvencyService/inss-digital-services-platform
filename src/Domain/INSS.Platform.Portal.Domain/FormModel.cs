using INSS.Platform.Portal.Domain.Exceptions;

namespace INSS.Platform.Portal.Domain;

public class FormModel : BaseModel
{
    public FormModel()
    {
        PathName = "tasks";
        Title = "Tasks";
    }

    public SectionModelCollection Sections { get; init; } = [];

    public bool CanSubmit => Sections.All(s => s.IsComplete);

    public string CurrentPageId { get; set; }

    public NavigationHistory History { get; } = new();
    
    public BaseModel GetCurrentPageFor(BaseModel model)
    {
        if (Id == model.Id && PageUrl == model.PageUrl)
        {
            return this;
        }

        BaseModel? page = Sections.GetCurrentPageFor(model);

        return page ?? this;
    }
    
    public BaseModel FindPage(string pageId)
    {
        if (Id == pageId)
        {
            return this;
        }
        
        BaseModel? page = Sections.FindPage(pageId);

        return page ?? this;
    }

    public AddAnotherModel GetAddAnother(string id)
    {
        if (FindPage(id) is not AddAnotherModel addAnother)
        {
            throw new FormModelException($"Unable to find the add another model associated to item {id}.");
        }
        
        return  addAnother;
    }
    
    public BaseModel FindNextPageAfter(BaseModel model)
    {
        BaseModel? nextPage = Sections.FindNextPageAfter(model);
        
        return nextPage ?? this;
    }
    
    public void Initialize()
    {
        PageUrl = $"/{PathName}";

        CurrentPageId = Id;
        
        foreach (SectionModel section in Sections)
        {
            section.PageUrl = $"{PageUrl}/{section.PathName}";

            foreach (BaseModel page in section.Pages)
            {
                page.PageUrl = $"{section.PageUrl}/{page.PathName}";

                if (page is AddAnotherModel addAnother && addAnother.Items.Count > 0)
                {
                    foreach (BaseModel itemPage in addAnother.Items[0])
                    {
                        itemPage.PageUrl = $"{page.PageUrl}/{itemPage.PathName}";
                    }
                }
            }

            section.PageUrl = $"{section.PageUrl}/summary";
        }
    }
}