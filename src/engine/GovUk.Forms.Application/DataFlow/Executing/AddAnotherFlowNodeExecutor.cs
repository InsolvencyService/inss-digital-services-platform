using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class AddAnotherFlowNodeExecutor : IFlowNodeExecutor
{
    private const int FirstWorkingPageNodeIdIndex = 0;
    private const int NextPageNodeIdIndex = 1;
    
    public ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        AddAnotherModel addAnother = context.CurrentPage.As<AddAnotherModel>();
        AddAnotherGroup groupInfo = context.Section.Pages.GetGroup<AddAnotherGroup>(addAnother.MetaData.Group);
        
        if (addAnother.AddAnotherItem)
        {
            addAnother.AddAnotherItem = false;

            foreach (PageModel workingPage in groupInfo.WorkingPages)
            {
                workingPage.Id = ContentId.New();
                workingPage.ClearValues();
            }

            return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[FirstWorkingPageNodeIdIndex]);
        }
        
        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[NextPageNodeIdIndex]);
    }
}