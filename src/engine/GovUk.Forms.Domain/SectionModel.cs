using System.Text.Json.Serialization;
using GovUk.Forms.Domain.Enums;

namespace GovUk.Forms.Domain;

public class SectionModel : ContentModel
{
    public string Title { get; init; } = string.Empty;

    public SectionStateTypes State { get; set; } = SectionStateTypes.NotStarted;

    public PageModelList Pages { get; init; } = [];

    [JsonIgnore]
    public PageModel FirstPage
    {
        get
        {
            if (State == SectionStateTypes.Completed)
            {
                return Pages.GetFirstOf<SummaryModel>();
            }
            
            return Pages[0] is GroupPageModel groupPage ? groupPage.Pages[0] : Pages[0];
        }
    }

    public void TransitionToInProgress()
    {
        if (State == SectionStateTypes.NotStarted)
        {
            State = SectionStateTypes.InProgress;
        }
    }
}