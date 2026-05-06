using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class SectionSummaryFlowNodeLoader : IFlowNodeLoader
{
    public ValueTask<NodeId?> LoadAsync(LoadContext context)
    {
        SummaryModel summary = context.Page.As<SummaryModel>();
        PageModelList savedPages = context.Section.Pages.GetCompletedPages();
        List<SummaryModel.SummaryInfo> overview = [];
        
        foreach (PageModel savedPage in savedPages)
        {
            if (savedPage is AddAnotherModel addAnother)
            {
                AddAnotherGroup groupInfo = context.Section.Pages.GetGroup<AddAnotherGroup>(addAnother.MetaData.Group);
                
                for (int i = 0; i < addAnother.Items.Count; i += groupInfo.WorkingPages.Count)
                {
                    string[] itemValues = addAnother.Items.Skip(i).Take(
                        groupInfo.WorkingPages.Count).SelectMany(p => p.GetSummaryInfo()).ToArray();
                    overview.Add(new SummaryModel.SummaryInfo
                    {
                        Title = addAnother.Title,
                        Values = itemValues,
                        ChangeUrl = addAnother.Path
                    });
                }
            }
            else
            {
                overview.Add(new SummaryModel.SummaryInfo
                {
                    Title = savedPage.Title,
                    Values = savedPage.GetSummaryInfo(),
                    ChangeUrl = context.Section.StartedDate is not null ? savedPage.Path : null
                });
            }
        }

        summary.Overview = overview.ToArray();

        return ValueTask.FromResult<NodeId?>(null);
    }
}