using Demo.GovUk.Forms.AboutYou.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Xunit;

namespace Demo.GovUk.Forms.AboutYou.Test.Application.DataFlow;

public class YourAgeFlowNodeExecutorTests
{
    [Fact]
    public async Task Below18_ExecuteAsync_ReturnsFalseNode()
    {
        YourAgeFlowNodeExecutor executor = new();
        FlowNode node = new() { Id = "NodeId1", PagePath = "/form/section/page1", NextNodes = ["NodeId2", "NodeId3"] };
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        SectionModel yourDetails = form.Sections["Your Details"];
        AgeModel age = yourDetails.Pages.GetFirstOf<AgeModel>();
        age.Value = 17;
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = yourDetails,
            UpdatedPage = age
        };
        
        NodeId? nextNodeId = await executor.ExecuteAsync(context);
        
        Assert.Equal(node.NextNodes[1], nextNodeId);
    }
    
    [Theory]
    [InlineData(18)]
    [InlineData(140)]
    public async Task EqualToOrAbove18_ExecuteAsync_ReturnsTrueNode(int value)
    {
        YourAgeFlowNodeExecutor executor = new();
        FlowNode node = new() { Id = "NodeId1", PagePath = "/form/section/page1", NextNodes = ["NodeId2", "NodeId3"] };
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        SectionModel yourDetails = form.Sections["Your Details"];
        AgeModel age = yourDetails.Pages.GetFirstOf<AgeModel>();
        age.Value = value;
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = yourDetails,
            UpdatedPage = age
        };
        
        NodeId? nextNodeId = await executor.ExecuteAsync(context);
        
        Assert.Equal(node.NextNodes[0], nextNodeId);
    }
}