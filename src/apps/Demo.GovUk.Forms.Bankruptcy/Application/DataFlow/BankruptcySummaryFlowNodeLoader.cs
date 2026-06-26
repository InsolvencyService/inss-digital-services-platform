using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace Demo.GovUk.Forms.Bankruptcy.Application.DataFlow;

public sealed class BankruptcySummaryFlowNodeLoader : SummaryFlowNodeLoader
{
    public override ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        DateModel bankruptcyDate = context.Section.Pages.GetFirstOf<DateModel>();
        SummaryModel summary = context.Section.Pages.GetFirstOf<SummaryModel>();

        SetReturnUrl(summary, context.Section.Pages.GetCompletedPages());
        
        List<SummaryCategoryDetail> details = [];
        
        AppendSummaryDetail(details, "Bankruptcy date", [bankruptcyDate.DateAsString], bankruptcyDate.Path);

        SummaryCategory aboutYouCategory = new() { Label = "Bankruptcy details", Details = details.ToArray() };

        summary.Categories = [aboutYouCategory];
        
        return ValueTask.FromResult<NodeId?>(null);
    }
}