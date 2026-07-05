using Demo.GovUk.Forms.AboutYou.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.PageFlow;
using GovUk.Forms.Domain;
using Xunit;

namespace Demo.GovUk.Forms.AboutYou.Test.Application.DataFlow;

public class YourSalaryPageExecutorTests
{
    [Fact]
    public async Task Below10000_ProcessAsync_SetsFalseNode()
    {
        YourSalaryPageExecutor executor = new();
        TreeNode node = new(new FlowNode { Id = "NodeId1", PagePath = "/form/section/page1", NextNodes = ["NodeId2", "NodeId3"] })
        {
            Children =
            [
                new TreeNode(new FlowNode { Id = "NodeId2", PagePath = "/form/section/page2" }),
                new TreeNode(new FlowNode { Id = "NodeId3", PagePath = "/form/section/page2" })
            ]
        };
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        SectionModel yourDetails = form.Sections["Your Details"];
        SalaryModel salary = yourDetails.Pages.GetFirstOf<SalaryModel>();
        salary.Value = 9_999;
        ExecutePageContext context = new()
        {
            CurrentNode = node,
            Form = form,
            Section = yourDetails,
            CurrentPage = salary
        };
        
        await executor.ExecuteAsync(context);
        
        Assert.Equal(1, context.ChildNodeIndex);
    }
    
    [Theory]
    [InlineData(10_000)]
    [InlineData(20_000)]
    public async Task EqualToOrAbove10000_ProcessAsync_SetsTrueNode(int value)
    {
        YourSalaryPageExecutor executor = new();
        TreeNode node = new(new FlowNode { Id = "NodeId1", PagePath = "/form/section/page1", NextNodes = ["NodeId2", "NodeId3"] })
        {
            Children =
            [
                new TreeNode(new FlowNode { Id = "NodeId2", PagePath = "/form/section/page2" }),
                new TreeNode(new FlowNode { Id = "NodeId3", PagePath = "/form/section/page2" })
            ]
        };
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        SectionModel yourDetails = form.Sections["Your Details"];
        SalaryModel salary = yourDetails.Pages.GetFirstOf<SalaryModel>();
        salary.Value = value;
        ExecutePageContext context = new()
        {
            CurrentNode = node,
            Form = form,
            Section = yourDetails,
            CurrentPage = salary
        };
        
        await executor.ExecuteAsync(context);
        
        Assert.Equal(0, context.ChildNodeIndex);
    }
}