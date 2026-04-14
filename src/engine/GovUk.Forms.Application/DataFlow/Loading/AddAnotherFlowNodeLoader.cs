using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class AddAnotherFlowNodeLoader : IFlowNodeLoader
{
    private const int FirstWorkingPageNodeIdIndex = 0;
    
    public ValueTask<NodeId?> LoadAsync(LoadContext context)
    {
        AddAnotherModel addAnother = context.Page.As<AddAnotherModel>();
        AddAnotherGroup groupInfo = context.Section.Pages.GetGroup<AddAnotherGroup>(addAnother.MetaData.Group);
            groupInfo.Remove.LinkedToNode = context.Nodes.First(n => n.PagePath == groupInfo.Remove.Path).Id; // TODO: Context helper
        
        if (addAnother.Items.Count == 0)
        {
            return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[FirstWorkingPageNodeIdIndex]);
        }
        
        List<AddAnotherModel.AddAnotherSummaryModel> summary = [];

        int setIndex = 0;
            
        for (int i = 0; i < addAnother.Items.Count; i += groupInfo.WorkingPages.Count)
        {
            string[] summaryInfo = addAnother.Items[i].GetSummaryInfo();
            
            summary.Add(new AddAnotherModel.AddAnotherSummaryModel
            {
                Value = string.Join(Environment.NewLine, summaryInfo),
                ChangeUrl = $"{groupInfo.CheckAnswers.Path}/?state={setIndex}",
                RemoveUrl = $"{groupInfo.Remove.Path}/?state={setIndex}"
            });

            setIndex++;
        }

        addAnother.SummaryInfo = summary.ToArray();
        addAnother.GroupLength = groupInfo.WorkingPages.Count;

        return ValueTask.FromResult<NodeId?>(null);
    }
}