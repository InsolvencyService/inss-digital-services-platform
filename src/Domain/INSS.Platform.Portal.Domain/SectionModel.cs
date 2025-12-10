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

    public BaseModel GetFirstPage()
    {
        BaseModel firstPage = Pages.First();
        
        if (firstPage is AddAnotherModel addAnother)
        {
            firstPage = addAnother.Pages.First();
        }

        return firstPage;
    }
}