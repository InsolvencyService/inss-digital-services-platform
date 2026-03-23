using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class AddAnotherWorkingPageFlowNodeExecutor : IFlowNodeExecutor
{
    public ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        AddAnotherGroup groupInfo = context.Section.Pages.GetGroup<AddAnotherGroup>(context.UpdatedPage.MetaData.Group);
        
        PageModel? existingItem = groupInfo.AddAnother.Items.FirstOrDefault(i => i.Id == context.UpdatedPage.Id);

        if (existingItem is not null)
        {
            int index = groupInfo.AddAnother.Items.IndexOf(existingItem);
            context.UpdatedPage.CopyTo(groupInfo.AddAnother.Items[index]);
        }
        else
        {
            PageModel copyOfPage = context.UpdatedPage.Clone();
            groupInfo.AddAnother.Items.Add(copyOfPage);    
        }

        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[0]);
    }
}