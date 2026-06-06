using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class SectionSummaryFlowNodeLoader : IFlowNodeLoader
{
    public ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        SummaryModel summary = context.CurrentPage.As<SummaryModel>();
        PageModelList savedPages = context.Section.Pages.GetCompletedPages();
        List<SummaryModel.SummaryInfo> overview = [];
        
        foreach (PageModel savedPage in savedPages)
        {
            savedPage.ReturnUrl = summary.Path;
            ContentPath changeUrl = savedPage is AddAnotherModel addAnother ? addAnother.Path : savedPage.Path;
            overview.Add(new SummaryModel.SummaryInfo
            {
                Title = savedPage.Title,
                Values = savedPage.GetSummaryInfo(),
                ChangeUrl = changeUrl
            });
        }

        summary.Overview = overview.ToArray();

        return ValueTask.FromResult<NodeId?>(null);
    }
}