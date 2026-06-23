using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

public abstract class SummaryFlowNodeLoader : IFlowNodeLoader
{
    public virtual ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        return ValueTask.FromResult<NodeId?>(null);
    }

    protected static void SetReturnUrl(SummaryModel summary, PageModelList pages)
    {
        foreach (PageModel page in pages)
        {
            page.ReturnUrl = summary.Path;
        }
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