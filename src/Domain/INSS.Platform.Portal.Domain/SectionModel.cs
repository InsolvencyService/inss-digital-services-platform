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
}