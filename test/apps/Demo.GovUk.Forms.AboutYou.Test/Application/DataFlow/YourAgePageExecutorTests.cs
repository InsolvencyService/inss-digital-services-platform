using Demo.GovUk.Forms.AboutYou.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.PageFlow;
using GovUk.Forms.Domain;
using Xunit;

namespace Demo.GovUk.Forms.AboutYou.Test.Application.DataFlow;

public class YourAgePageExecutorTests
{
    [Fact]
    public async Task Below18_ExecuteAsync_SetsFalseNode()
    {
        YourAgePageExecutor executor = new();
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
        AgeModel age = yourDetails.Pages.GetFirstOf<AgeModel>();
        age.Value = 17;
        ExecutePageContext context = new()
        {
            CurrentNode = node,
            Form = form,
            Section = yourDetails,
            CurrentPage = age
        };
        
        await executor.ExecuteAsync(context);
        
        Assert.Equal(1, context.ChildNodeIndex);
    }
    
    [Theory]
    [InlineData(18)]
    [InlineData(140)]
    public async Task EqualToOrAbove18_ExecuteAsync_SetsTrueNode(int value)
    {
        YourAgePageExecutor executor = new();
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
        AgeModel age = yourDetails.Pages.GetFirstOf<AgeModel>();
        age.Value = value;
        ExecutePageContext context = new()
        {
            CurrentNode = node,
            Form = form,
            Section = yourDetails,
            CurrentPage = age
        };
        
        await executor.ExecuteAsync(context);
        
        Assert.Equal(0, context.ChildNodeIndex);
    }
}