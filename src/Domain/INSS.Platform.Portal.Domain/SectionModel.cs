namespace INSS.Platform.Portal.Domain;

public class SectionModel : BaseModel
{
    public SectionModel()
    {
        PathName = "section";
        Name = "Section";
    }

    //public bool IsComplete { get; set; }

    public SectionProgressType Progress { get; set; }
    
    public BaseModel[] Pages { get; init; } = [];

    public BaseModel GetFirstPage()
    {
        BaseModel firstPage = Pages.First();
        
        if (firstPage is AddAnotherModel addAnother)
        {
            firstPage = addAnother.GetFirstPage();//.Pages.First();
        }

        return firstPage;
    }
}

public enum SectionProgressType
{
    NotStarted,
    InProgress,
    Completed
}