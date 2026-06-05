using System.Globalization;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class AddAnotherRemoveFlowNodeLoader : IFlowNodeLoader
{
    public ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        RemoveModel remove = context.CurrentPage.As<RemoveModel>();
        AddAnotherGroup groupInfo = context.Section.Pages.GetGroup<AddAnotherGroup>(remove.MetaData.Group);
        remove.SetIndex = int.Parse(context.State!, CultureInfo.InvariantCulture);
        remove.ReturnUrl = groupInfo.AddAnother.Path;
        PageModel[] subItems = groupInfo.AddAnother.Items.Skip(remove.SetIndex * groupInfo.WorkingPages.Count).Take(groupInfo.WorkingPages.Count).ToArray();
        PageModel firstPageForRemoving = subItems[0];
        string[] summaryInfo = firstPageForRemoving.GetSummaryInfo();
        remove.RemoveQuestion = $"Do you want to remove {string.Join(Environment.NewLine, summaryInfo)}?";

        return ValueTask.FromResult<NodeId?>(null);
    }
}