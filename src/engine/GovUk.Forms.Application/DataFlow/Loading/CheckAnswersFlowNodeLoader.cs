using GovUk.Forms.Application.Exceptions;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class CheckAnswersFlowNodeLoader : IFlowNodeLoader
{
    public ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        CheckAnswersModel checkAnswers = context.CurrentPage.As<CheckAnswersModel>();
        AddAnotherGroup groupInfo = context.Section.Pages.GetGroup<AddAnotherGroup>(checkAnswers.MetaData.Group);

        int? setIndex = context.GetQueryParam<int?>("index");
        
        if (setIndex is not null)
        {
            PageModel[] pages = groupInfo.AddAnother.Items.Skip(
                setIndex.Value * groupInfo.WorkingPages.Count).Take(groupInfo.WorkingPages.Count).ToArray();
            
            if (groupInfo.WorkingPages.Count != pages.Length)
            {
                throw new FlowchartException("The expected working pages not not match edit Ids.");
            }
            
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i].CopyTo(groupInfo.WorkingPages[i]);
                groupInfo.WorkingPages[i].Id = pages[i].Id;
            }
        }
        
        List<CheckAnswersItem> itemList = [];
        context.Section.ReturnUrl = checkAnswers.Path;
        
        foreach (PageModel groupPage in groupInfo.WorkingPages)
        {
            string[] values = groupPage.GetSummaryInfo();
            itemList.Add(new CheckAnswersItem
            {
                Title = groupPage.Title,
                ChangeUrl = groupPage.Path,
                Values = values
            });
        }

        checkAnswers.Items = itemList.ToArray();

        return ValueTask.FromResult<NodeId?>(null);
    }
}