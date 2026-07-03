using Demo.GovUk.Forms.AboutYou.Application.DataFlow;
using Demo.GovUk.Forms.AboutYou.Domain;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Domain;
using Xunit;

namespace Demo.GovUk.Forms.AboutYou.Test.Application.DataFlow;

public class OwnHomePageExecutorTests
{
    [Fact]
    public async Task DoesNotOwnHome_ExecuteAsync_SetsFalseNode()
    {
        OwnHomePageExecutor executor = new();
        TreeNode node = new(new FlowNode { Id = "NodeId1", PagePath = "/form/section/page1", NextNodes = ["NodeId2", "NodeId3"] })
        {
            Children =
            [
                new TreeNode(new FlowNode { Id = "NodeId2", PagePath = "/form/section/page2" }),
                new TreeNode(new FlowNode { Id = "NodeId3", PagePath = "/form/section/page2" })
            ]
        };
        FormModel form = TestFormModels.CreateWithYourAssetsSection();
        SectionModel yourAssets = form.Sections["Your Assets"];
        OwnHomeModel ownHome = yourAssets.Pages.GetFirstOf<OwnHomeModel>();
        ownHome.OwnsHome = false;
        ExecutePageContext context = new()
        {
            CurrentNode = node,
            Form = form,
            Section = yourAssets,
            CurrentPage = ownHome
        };
        await executor.ExecuteAsync(context);
        
        Assert.Equal(1, context.ChildNodeIndex);
    }
    
    [Fact]
    public async Task DoesOwnHome_ProcessAsync_SetsTrueNode()
    {
        OwnHomePageExecutor executor = new();
        TreeNode node = new(new FlowNode { Id = "NodeId1", PagePath = "/form/section/page1", NextNodes = ["NodeId2", "NodeId3"] })
            {
                Children =
                [
                    new TreeNode(new FlowNode { Id = "NodeId2", PagePath = "/form/section/page2" }),
                    new TreeNode(new FlowNode { Id = "NodeId3", PagePath = "/form/section/page2" })
                ]
            };
        FormModel form = TestFormModels.CreateWithYourAssetsSection();
        SectionModel yourAssets = form.Sections["Your Assets"];
        OwnHomeModel ownHome = yourAssets.Pages.GetFirstOf<OwnHomeModel>();
        ownHome.OwnsHome = true;
        ExecutePageContext context = new()
        {
            CurrentNode = node,
            Form = form,
            Section = yourAssets,
            CurrentPage = ownHome
        };

        await executor.ExecuteAsync(context);
        
        Assert.Equal(0, context.ChildNodeIndex);
    }
}