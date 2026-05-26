using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace Demo.GovUk.Forms.ContactUs.Application.DataFlow;

public sealed class AddAnotherUploadFilesFlowNodeLoader : AddAnotherFlowNodeLoader
{
    public override async ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        NodeId? nextNodeId = await base.LoadAsync(context);
        AddAnotherModel addAnother = context.CurrentPage.As<AddAnotherModel>();
        addAnother.CanAddAnother = addAnother.Items.Count < 2;
        return nextNodeId;
    }
}