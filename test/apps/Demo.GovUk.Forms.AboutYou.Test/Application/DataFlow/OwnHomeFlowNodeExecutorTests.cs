using Demo.GovUk.Forms.AboutYou.Application.DataFlow;
using Demo.GovUk.Forms.AboutYou.Domain;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Xunit;

namespace Demo.GovUk.Forms.AboutYou.Test.Application.DataFlow;

public class OwnHomeFlowNodeExecutorTests
{
    [Fact]
    public async Task DoesNotOwnHome_ExecuteAsync_ReturnsFalseNode()
    {
        OwnHomeFlowNodeExecutor executor = new();
        FlowNode node = new() { Id = "NodeId1", PagePath = "/form/section/page1", NextNodes = ["NodeId2", "NodeId3"] };
        FormModel form = TestFormModels.CreateWithYourAssetsSection();
        SectionModel yourAssets = form.Sections["Your Assets"];
        OwnHomeModel ownHome = yourAssets.Pages.GetFirstOf<OwnHomeModel>();
        ownHome.OwnsHome = false;
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = yourAssets,
            CurrentPage = ownHome
        };
        NodeId? nextNodeId = await executor.ExecuteAsync(context);
        
        Assert.Equal(node.NextNodes[1], nextNodeId);
    }
    
    [Fact]
    public async Task DoesOwnHome_ProcessAsync_ReturnsTrueNode()
    {
        OwnHomeFlowNodeExecutor executor = new();
        FlowNode node = new() { Id = "NodeId1", PagePath = "/form/section/page1", NextNodes = ["NodeId2", "NodeId3"] };
        FormModel form = TestFormModels.CreateWithYourAssetsSection();
        SectionModel yourAssets = form.Sections["Your Assets"];
        OwnHomeModel ownHome = yourAssets.Pages.GetFirstOf<OwnHomeModel>();
        ownHome.OwnsHome = true;
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = yourAssets,
            CurrentPage = ownHome
        };

        NodeId? nextNodeId = await executor.ExecuteAsync(context);
        
        Assert.Equal(node.NextNodes[0], nextNodeId);
    }
}