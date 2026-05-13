using System.Text.Json.Serialization;

namespace GovUk.Forms.Domain;

public class SectionModel : ContentModel
{
    public string Title { get; init; } = string.Empty;
    
    public PageModelList Pages { get; init; } = [];

    public DateTimeOffset? StartedDate { get; set; }
    
    public DateTimeOffset? CompletedDate { get; set; }
    
    [JsonIgnore]
    public PageModel FirstPage
    {
        get
        {
            if (CompletedDate is not null)
            {
                return Pages.GetFirstOf<SummaryModel>();
            }
            
            return Pages[0] is GroupPageModel groupPage ? groupPage.Pages[0] : Pages[0];
        }
    }
    
    public void SetInProgress()
    {
        StartedDate ??= DateTimeOffset.Now;
    }

    public void SetCompleted()
    {
        CompletedDate = DateTimeOffset.Now;
    }
}