using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class FileUploadFlowNodeLoaderTests
{
    private const string? NoState = null;
    
    [Fact]
    public async Task LoadingFileUploadIfDeclarationNotAccepted_LoadAsync_ThrowsException()
    {
        FileUploadFlowNodeLoader loader = new();
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel ipUploadSection = form.Sections["IP Upload"];
        IPUploadDeclarationModel declaration = ipUploadSection.Pages.GetFirstOf<IPUploadDeclarationModel>();
        declaration.Accepted = false;
        FlowNode node = new() { Id = "NodeId1", PagePath = declaration.Path, NextNodes = ["NodeId2"] };
        LoadContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = ipUploadSection,
            Page = declaration,
            State = NoState
        };
        
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await loader.LoadAsync(context));
        
        Assert.Equal("You must accept the declaration.", exception.Message);
    }
    
    [Fact]
    public async Task LoadingFileUploadIfDeclarationAccepted_LoadAsync_DoesNotThrowsException()
    {
        FileUploadFlowNodeLoader loader = new();
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel ipUploadSection = form.Sections["IP Upload"];
        IPUploadDeclarationModel declaration = ipUploadSection.Pages.GetFirstOf<IPUploadDeclarationModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = declaration.Path, NextNodes = ["NodeId2"] };
        declaration.Accepted = true;

        try
        {
            LoadContext context = new()
            {
                Nodes = [node],
                CurrentNode = node,
                Form = form,
                Section = ipUploadSection,
                Page = declaration,
                State = NoState
            };
            await loader.LoadAsync(context);
        }
        catch (Exception error)
        {
            Assert.Fail(error.Message);
        }
    }
}