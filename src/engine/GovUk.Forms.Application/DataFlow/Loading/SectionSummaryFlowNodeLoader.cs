using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class SectionSummaryFlowNodeLoader : IFlowNodeLoader
{
    public ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        SummaryModel summary = context.CurrentPage.As<SummaryModel>();
        PageModelList savedPages = context.Section.Pages.GetCompletedPages();
        List<CheckAnswersItem> items = [];
        
        foreach (PageModel savedPage in savedPages)
        {
            savedPage.ReturnUrl = summary.Path;
            ContentPath changeUrl = savedPage is AddAnotherModel addAnother ? addAnother.Path : savedPage.Path;
            items.Add(new CheckAnswersItem
            {
                Title = savedPage.Title,
                Values = savedPage.GetSummaryInfo(),
                ChangeUrl = changeUrl
            });
        }

        summary.Items = items.ToArray();

        return ValueTask.FromResult<NodeId?>(null);
    }
}