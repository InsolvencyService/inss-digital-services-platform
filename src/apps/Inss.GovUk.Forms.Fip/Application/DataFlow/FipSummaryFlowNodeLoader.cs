using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace Inss.GovUk.Forms.Fip.Application.DataFlow;

public sealed class FipSummaryFlowNodeLoader : SummaryFlowNodeLoader
{
    public override ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        // TODO: Align this with your actual journey and pages defined (if required)
        
        DateModel bankruptcyDate = context.Section.Pages.GetFirstOf<DateModel>();
        SummaryModel summary = context.Section.Pages.GetFirstOf<SummaryModel>();

        context.Section.ReturnUrl = summary.Path;
        
        List<SummaryCategoryDetail> details = [];
        
        AppendSummaryDetail(details, "Bankruptcy date", [bankruptcyDate.DateAsString], bankruptcyDate.Path);

        SummaryCategory aboutYouCategory = new() { Label = "Bankruptcy details", Details = details.ToArray() };

        summary.Categories = [aboutYouCategory];
        
        return ValueTask.FromResult<NodeId?>(null);
    }
}