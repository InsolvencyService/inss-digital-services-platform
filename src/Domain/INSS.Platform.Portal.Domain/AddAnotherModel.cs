using INSS.Platform.Portal.Domain.Attributes;

namespace INSS.Platform.Portal.Domain;

public sealed class AddAnotherModel : BaseModel
{
    public AddAnotherModel()
    {
        PathName = "add-another";
        Title = "Add Another";
    }
    
    public ItemCollection Items { get; init; } = [];
    
    public bool AddAnotherItem { get; set; }
    
    [ExcludeFromSummary]
    public string? QuestionText { get; init; }
    
    [ExcludeFromSummary]
    public string? QuestionHint { get; init; }

    public string GetChangeUrl(BaseModel item)
    {
        return $"{PageUrl}/change/?itemId={item.Id}";
    }
    
    public string GetRemoveUrl(BaseModel item)
    {
        return $"{PageUrl}/remove/?itemId={item.Id}";
    }
}