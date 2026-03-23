using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class AddAnotherRemoveFlowNodeExecutor : IFlowNodeExecutor
{
    public ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        RemoveModel remove = context.UpdatedPage.As<RemoveModel>();

        if (remove.RemoveConfirmed)
        {
            AddAnotherGroup groupInfo = context.Section.Pages.GetGroup<AddAnotherGroup>(remove.MetaData.Group);
            AddAnotherModel addAnother = groupInfo.AddAnother;
            PageModel[] subItems = addAnother.Items.Skip(remove.SetIndex * groupInfo.WorkingPages.Count).Take(groupInfo.WorkingPages.Count).ToArray();
            bool hasRemovedItems = addAnother.Items.RemoveMatchingPages(subItems.Select(i => i.Id).ToArray());

            if (hasRemovedItems)
            {
                ClearGroupWorkingPageValues(context.Section, addAnother);
            }

            groupInfo.Remove.ResetRemove();
        }

        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[0]);
    }
    
    private static void ClearGroupWorkingPageValues(SectionModel section, AddAnotherModel addAnother)
    {
        AddAnotherGroup groupInfo = section.Pages.GetGroup<AddAnotherGroup>(addAnother.MetaData.Group);

        foreach (PageModel groupPage in groupInfo.WorkingPages)
        {
            groupPage.ClearValues();
        }
    }
}