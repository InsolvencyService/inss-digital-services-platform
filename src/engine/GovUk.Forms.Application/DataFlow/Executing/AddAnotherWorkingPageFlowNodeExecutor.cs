using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class AddAnotherWorkingPageFlowNodeExecutor : IFlowNodeExecutor
{
    public ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        AddAnotherGroup groupInfo = context.Section.Pages.GetGroup<AddAnotherGroup>(context.CurrentPage.MetaData.Group);
        
        PageModel? existingItem = groupInfo.AddAnother.Items.FirstOrDefault(i => i.Id == context.CurrentPage.Id);

        if (existingItem is not null)
        {
            int index = groupInfo.AddAnother.Items.IndexOf(existingItem);
            context.CurrentPage.CopyTo(groupInfo.AddAnother.Items[index]);
        }
        else
        {
            PageModel copyOfPage = context.CurrentPage.Clone();
            groupInfo.AddAnother.Items.Add(copyOfPage);    
        }

        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[0]);
    }
}