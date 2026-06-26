using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class PostSubmitFlowNodeExecutorTests
{
    private readonly PostSubmitFlowNodeExecutor _postSubmitFlowNodeExecutor = new();
    
    [Fact]
    public async Task ForPostSubmit_ExecuteAsync_ReturnsFirstNode()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel section = form.Sections["IP Upload"];
        section.Track("NodeId1");
        PostSubmitModel postSubmit = section.Pages.GetFirstOf<PostSubmitModel>();
        FlowNode node = new() { Id = "NodeId2", PagePath = postSubmit.Path, NextNodes = ["NodeId3"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            CurrentPage = postSubmit
        };
        
        NodeId? nextNodeId = await _postSubmitFlowNodeExecutor.ExecuteAsync(context);

        Assert.NotNull(nextNodeId);
        Assert.Equal("NodeId3", nextNodeId);
    }
    
    [Fact]
    public async Task ForPostSubmit_ExecuteAsync_ResetsVisitedNodes()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel section = form.Sections["IP Upload"];
        section.Track("NodeId1");
        PostSubmitModel postSubmit = section.Pages.GetFirstOf<PostSubmitModel>();
        FlowNode node = new() { Id = "NodeId2", PagePath = postSubmit.Path, NextNodes = ["NodeId3"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            CurrentPage = postSubmit
        };
        
        await _postSubmitFlowNodeExecutor.ExecuteAsync(context);

        Assert.Empty(section.VisitedNodes);
    }
}