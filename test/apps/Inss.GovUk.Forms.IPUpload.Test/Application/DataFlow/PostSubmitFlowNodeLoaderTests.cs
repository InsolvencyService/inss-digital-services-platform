using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class PostSubmitFlowNodeLoaderTests
{
    private readonly PostSubmitFlowNodeLoader _postSubmitFlowNodeLoader;
    private readonly IUserFormService _userFormService;
    
    public PostSubmitFlowNodeLoaderTests()
    {
        _userFormService = Substitute.For<IUserFormService>();
        _postSubmitFlowNodeLoader = new PostSubmitFlowNodeLoader(_userFormService);
    }
    
    [Fact]
    public async Task LoadingPostSubmit_ExecuteAsync_CallsUserServiceRemove()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel section = form.Sections["IP Upload"];
        PostSubmitModel postSubmit = section.Pages.GetFirstOf<PostSubmitModel>();
        FlowNode node = new() { Id = "NodeId5", PagePath = postSubmit.Path, NextNodes = ["NodeId1"] };
        LoadContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            Page = postSubmit
        };
        
        await _postSubmitFlowNodeLoader.LoadAsync(context);
        
        await _userFormService.Received(1).RemoveAsync(context.Form);
    }
    
    [Fact]
    public async Task LoadingPostSubmit_LoadAsync_ResetsPreviousPageToNull()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel section = form.Sections["IP Upload"];
        PostSubmitModel postSubmit = section.Pages.GetFirstOf<PostSubmitModel>();
        FlowNode node = new() { Id = "NodeId5", PagePath = postSubmit.Path, NextNodes = ["NodeId1"] };
        LoadContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            Page = postSubmit
        };

        await _postSubmitFlowNodeLoader.LoadAsync(context);
        
        Assert.Null(postSubmit.PreviousPagePath);
    }
}