using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class SectionSummaryFlowNodeExecutor : IFlowNodeExecutor
{
    public ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        context.Section.State = SectionStateTypes.Completed;
        
        foreach (PageModel page in context.Section.Pages)
        {
            page.EditMode |= PageEditTypes.Locked;
            page.PreviousPagePath = context.Form.Path;
        }
        
        return ValueTask.FromResult<NodeId?>(null);
    }
}