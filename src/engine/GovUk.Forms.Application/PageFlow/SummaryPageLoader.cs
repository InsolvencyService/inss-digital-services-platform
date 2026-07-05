using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.PageFlow;

public abstract class SummaryPageLoader : IPageLoader
{
    public virtual ValueTask LoadAsync(LoadPageContext context)
    {
        return ValueTask.CompletedTask;
    }
    
    protected static void AppendSummaryDetail(List<SummaryCategoryDetail> details, string label, string[] values, ContentPath? change = null)
    {
        List<SummaryAction> actions = [];
        
        if (change is not null)
        {
            actions.Add(new SummaryAction { Label = "Change", Url = change });
        }
        
        SummaryCategoryDetail detail = new() { Label = label, Values = values, Actions = actions.ToArray() };
        
        details.Add(detail);
    }
}