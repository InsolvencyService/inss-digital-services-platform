namespace INSS.Platform.Portal.Domain;

public sealed class AddAnotherModel : BaseModel
{
    public AddAnotherModel()
    {
        PathName = "add-another";
        Name = "Add Another";
    }
    
    public ItemCollection Items { get; init; } = [];
    
    public bool AddAnotherItem { get; set; }
    
    public string? QuestionText { get; init; }
    
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