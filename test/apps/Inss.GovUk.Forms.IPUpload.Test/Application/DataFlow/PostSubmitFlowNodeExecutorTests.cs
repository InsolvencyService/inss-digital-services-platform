using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class PostSubmitFlowNodeExecutorTests
{
    private readonly PostSubmitFlowNodeExecutor _postSubmitFlowNodeExecutor;
    private readonly IUserFormService _userFormService;
    
    public PostSubmitFlowNodeExecutorTests()
    {
        _userFormService = Substitute.For<IUserFormService>();
        _postSubmitFlowNodeExecutor = new PostSubmitFlowNodeExecutor(_userFormService);
    }
    
    [Fact]
    public async Task ForForm_ExecuteAsync_CallsUserServiceRemove()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel section = form.Sections["IP Upload"];
        PostSubmitModel postSubmit = section.Pages.GetFirstOf<PostSubmitModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = postSubmit.Path, NextNodes = ["NodeId2"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            FinalExecuteStep = true
        };
        
        await _postSubmitFlowNodeExecutor.ExecuteAsync(context);
        
        await _userFormService.Received(1).RemoveAsync(context.Form);
    }
    
    [Fact]
    public async Task ForForm_ExecuteAsync_ReturnsNextNode()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel section = form.Sections["IP Upload"];
        PostSubmitModel postSubmit = section.Pages.GetFirstOf<PostSubmitModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = postSubmit.Path, NextNodes = ["NodeId2"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            FinalExecuteStep = true
        };
        
        NodeId? nextNodeId = await _postSubmitFlowNodeExecutor.ExecuteAsync(context);
        
        Assert.Equal(node.NextNodes[0], nextNodeId);
    }
}