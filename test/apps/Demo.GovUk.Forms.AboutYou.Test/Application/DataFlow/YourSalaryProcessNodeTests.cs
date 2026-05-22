using Demo.GovUk.Forms.AboutYou.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Xunit;

namespace Demo.GovUk.Forms.AboutYou.Test.Application.DataFlow;

public class YourSalaryFlowNodeExecutorTests
{
    [Fact]
    public async Task Below10000_ProcessAsync_ReturnsFalseNode()
    {
        YourSalaryFlowNodeExecutor processor = new();
        FlowNode node = new() { Id = "NodeId1", PagePath = "/form/section/page1", NextNodes = ["NodeId2", "NodeId3"] };
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        SectionModel yourDetails = form.Sections["Your Details"];
        SalaryModel salary = yourDetails.Pages.GetFirstOf<SalaryModel>();
        salary.Value = 9_999;
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = yourDetails,
            CurrentPage = salary
        };
        
        NodeId? nextNodeId = await processor.ExecuteAsync(context);
        
        Assert.Equal(node.NextNodes[1], nextNodeId);
    }
    
    [Theory]
    [InlineData(10_000)]
    [InlineData(20_000)]
    public async Task EqualToOrAbove10000_ProcessAsync_ReturnsTrueNode(int value)
    {
        YourSalaryFlowNodeExecutor processor = new();
        FlowNode node = new() { Id = "NodeId1", PagePath = "/form/section/page1", NextNodes = ["NodeId2", "NodeId3"] };
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        SectionModel yourDetails = form.Sections["Your Details"];
        SalaryModel salary = yourDetails.Pages.GetFirstOf<SalaryModel>();
        salary.Value = value;
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = yourDetails,
            CurrentPage = salary
        };
        
        NodeId? nextNodeId = await processor.ExecuteAsync(context);
        
        Assert.Equal(node.NextNodes[0], nextNodeId);
    }
}