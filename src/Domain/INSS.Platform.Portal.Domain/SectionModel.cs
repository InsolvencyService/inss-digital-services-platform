using INSS.Platform.Portal.Domain.Exceptions;

namespace INSS.Platform.Portal.Domain;

public class SectionModel : BaseModel
{
    public SectionModel()
    {
        PathName = "section";
        Name = "Section";
    }

    public bool IsComplete { get; set; }
    
    public PageCollection Pages { get; init; } = [];

    public BaseModel GetStartPage()
    {
        if (Pages.Count == 0)
        {
            throw new FormModelException("There are no pages defined.");
        }
        
        if (Pages[0] is AddAnotherModel addAnother)
        {
            return addAnother.Items[0][0];
        }

        return Pages[0];
    }
    
    public string GetChangeUrl(BaseModel item)
    {
        return $"{PageUrl}/change/?itemId={item.Id}";
    }
    
    public string GetStartUrl()
    {
        return $"{PageUrl}/start";
    }
}