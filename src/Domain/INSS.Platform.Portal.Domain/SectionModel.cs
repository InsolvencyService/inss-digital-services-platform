using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace INSS.Platform.Portal.Domain;

public class SectionModel : BaseModel
{
    public SectionModel()
    {
        PathName = "section";
        Name = "Section";
    }

    public bool IsComplete { get; set; }
    
    public BaseModel[] Pages { get; init; } = [];
    
    [ValidateNever]
    public SectionContext Context { get; } = new();
    
    public string GetChangeUrl(BaseModel item)
    {
        return $"{PageUrl}/change/?itemId={item.Id}";
    }
}